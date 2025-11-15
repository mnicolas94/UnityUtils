using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace Utils.Profiling
{
    public class FpsCounter : MonoBehaviour
    {
        [SerializeField] private GameObject canvas;
        [SerializeField] private TextMeshProUGUI uiText;
        [SerializeField] private int countFrames;
        
        private List<float> _lastFramesDeltas;

        private void Start()
        {
            _lastFramesDeltas = new List<float>();
        }

        private void Update()
        {
            AddDelta();
            float fps = ComputeFps();
            UpdateUi(fps);
        }

        public void Toggle()
        {
            bool currentState = enabled;
            enabled = !currentState;
            canvas.SetActive(!currentState);
        }

        private void AddDelta()
        {
            _lastFramesDeltas.Add(Time.deltaTime);
            if (_lastFramesDeltas.Count > countFrames)
            {
                _lastFramesDeltas.RemoveAt(0);
            }
        }

        private float ComputeFps()
        {
            float sum = 0;
            foreach (var delta in _lastFramesDeltas)
            {
                sum += delta;
            }

            float secondsPerFrame = sum / _lastFramesDeltas.Count;
            float fps = 1 / secondsPerFrame;

            return fps;
        }

        private void UpdateUi(float fps)
        {
            uiText.text = $"{fps:0.000}";
        }
    }
}