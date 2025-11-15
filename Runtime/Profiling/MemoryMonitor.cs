using UnityEngine;
using UnityEngine.Profiling;
using System;
using TMPro;
using UnityEngine.UI;
using Utils.Collections;

namespace Utils.Profiling
{
    // source: https://gist.github.com/adammyhre/51aed0c1168ed7fcedf3d6ca6f5c036b (git-amend)
    public class MemoryMonitor : MonoBehaviour
    {
#region Fields

        public TextMeshProUGUI allocatedRamText, reservedRamText, monoRamText, gcCountText;
        public RawImage memoryGraphImage;
        public int historyLength = 300, graphHeight = 100;
    
        public Color32 allocatedColor = new(0, 255, 0, 255),
            monoColor = new(0, 150, 255, 255),
            reservedColor = new(200, 200, 200, 255),
            gcEventColor = new(255, 0, 0, 255);
    
        const float BYTES_TO_MB = 1024f * 1024f;
        static readonly Color32 backgroundColor = new(0, 0, 0, 255);
    
        CircularBuffer<long> allocated, reserved, mono, gcAlloc;
        CircularBuffer<bool> gcEvents;
        Texture2D graphTexture;
        Color32[] pixels;
    
        Recorder rec;
        int lastGCCount;
        long maxReserved;

        #endregion

        void Start()
        {
            allocated = new(historyLength);
            reserved = new(historyLength);
            mono = new(historyLength);
            gcAlloc = new(historyLength);
            gcEvents = new(historyLength);
        
            rec = Recorder.Get("GC.Alloc");
            rec.enabled = false;
            rec.FilterToCurrentThread();
            rec.enabled = true;
        
            graphTexture = new Texture2D(historyLength, graphHeight + 1, TextureFormat.RGBA32, false)
                { wrapMode = TextureWrapMode.Clamp };
            pixels = new Color32[graphTexture.width * graphTexture.height];
            memoryGraphImage.texture = graphTexture;
        }

        void Update()
        {
            long allocBytes = Profiler.GetTotalAllocatedMemoryLong();
            long resBytes = Profiler.GetTotalReservedMemoryLong();
            long monoBytes = Profiler.GetMonoUsedSizeLong();
            long gcAllocs = rec.sampleBlockCount;
        
            int gcCount = GC.CollectionCount(0); // Only one generation in Unity at index 0
            bool gcHappened = gcCount != lastGCCount;
            lastGCCount = gcCount;
        
            UpdateText(allocatedRamText, allocBytes / BYTES_TO_MB, allocatedColor);
            UpdateText(reservedRamText, resBytes / BYTES_TO_MB, reservedColor);
            UpdateText(monoRamText, monoBytes / BYTES_TO_MB, monoColor);
            UpdateText(gcCountText, gcCount, gcHappened ? Color.red : Color.white);
        
            if (resBytes > maxReserved) maxReserved = resBytes;
        
            allocated.Enqueue(allocBytes);
            reserved.Enqueue(resBytes);
            mono.Enqueue(monoBytes);
            gcAlloc.Enqueue(gcAllocs);
            gcEvents.Enqueue(gcHappened);
        
            DrawGraph();
        }

        void DrawGraph()
        {
            if (!graphTexture) return;
        
            Array.Fill(pixels, backgroundColor);
            int width = graphTexture.width;

            for (int i = 0; i < allocated.Count; i++)
            {
                int x = i;
                float scale = graphHeight / (float)Math.Max(maxReserved, 1);
                int hMono = (int)(mono[i] * scale);
                int hAlloc = (int)(allocated[i] * scale);
                int hRes = (int)(reserved[i] * scale);

                for (int y = 0; y < hRes; y++)
                {
                    int idx = x + y * width;
                    pixels[idx] = y < hMono ? monoColor :
                        y < hAlloc ? allocatedColor : reservedColor;
                }

                if (gcEvents[i])
                {
                    for (int y = 0; y < graphHeight; y++)
                    {
                        pixels[x + y * width] = gcEventColor;
                    }
                }
            }

            if (gcEvents.Count > 2 && gcEvents[gcEvents.Count - 1])
            {
                long monoDiff = mono[mono.Count - 2] - mono[mono.Count - 1];
                Debug.Log($"GC Event detected! Mono memory change: {monoDiff / BYTES_TO_MB:F2} MB");
            }
        
            graphTexture.SetPixels32(pixels);
            graphTexture.Apply(false);
        }

        void UpdateText(TMPro.TextMeshProUGUI text, float value, Color color)
        {
            if (!text) return;
            text.text = $"{value:F1} MB";
            text.color = color;
        }
    }
}