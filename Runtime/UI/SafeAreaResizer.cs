using System;
using UnityEditor;
using UnityEngine;
using UnityEngine.Serialization;
using Utils.Attributes;
using Screen = UnityEngine.Device.Screen;

namespace Utils.UI
{
    public class SafeAreaResizer : MonoBehaviour
    {
        [FormerlySerializedAs("target"), SerializeField, AutoProperty]
        private RectTransform _target;

        private Rect _lastSafeArea;
        
        private void Start()
        {
            var rect = Screen.safeArea;
            ResizeToSafeArea(rect);
        }

        private void Update()
        {
            if (_lastSafeArea != Screen.safeArea)
            {
                ResizeToSafeArea(Screen.safeArea);
            }
        }

        private void ResizeToSafeArea(Rect rect)
        {
            var anchorMin = rect.min;
            var anchorMax = rect.max;

            anchorMin.x /= Screen.width;
            anchorMin.y /= Screen.height;
            anchorMax.x /= Screen.width;
            anchorMax.y /= Screen.height;

            _target.anchorMin = anchorMin;
            _target.anchorMax = anchorMax;

            _lastSafeArea = rect;
        }
        
#if UNITY_EDITOR

        [Space] public Rect _rect;

        [ContextMenu("Set test rect")]
        public void ChangeGlobalSafeAreaRect()
        {
            SafeAreaResizerEditorUtils.GlobalTestSafeAreaRect = _rect;
        }

        [ContextMenu("Resize to safe area")]
        public void Resize()
        {
            var r = SafeAreaResizerEditorUtils.GlobalTestSafeAreaRect;
            r.x *= Screen.width;
            r.y *= Screen.height;
            r.width *= Screen.width;
            r.height *= Screen.height;
            ResizeToSafeArea(r);
        }
        
#endif
    }
    
#if UNITY_EDITOR
    public static class SafeAreaResizerEditorUtils
    {
        public static Rect GlobalTestSafeAreaRect = new Rect(0, 0.05f, 1, 0.9f);
        
        [MenuItem("Tools/Facticus/Utils/Ui/Resize all safe area resizers")]
        public static void ResizeAll()
        {
            var allResizers = GameObject.FindObjectsOfType<SafeAreaResizer>();
            foreach (var resizer in allResizers)
            {
                resizer.Resize();
            }
        }
    }
#endif
}