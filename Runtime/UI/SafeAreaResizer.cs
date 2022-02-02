using UnityEditor;
using UnityEngine;
using Screen = UnityEngine.Device.Screen;

namespace Utils.UI
{
    public class SafeAreaResizer : MonoBehaviour
    {
        [SerializeField] private RectTransform target;
        
        private void Start()
        {
            var rect = Screen.safeArea;
            ResizeToSafeArea(rect);
        }

        private void ResizeToSafeArea(Rect rect)
        {
            var anchorMin = rect.min;
            var anchorMax = rect.max;

            anchorMin.x /= Screen.width;
            anchorMin.y /= Screen.height;
            anchorMax.x /= Screen.width;
            anchorMax.y /= Screen.height;

            target.anchorMin = anchorMin;
            target.anchorMax = anchorMax;
        }
        
#if UNITY_EDITOR

        [Space] public Rect rect;

        [ContextMenu("Set test rect")]
        public void ChangeGlobalSafeAreaRect()
        {
            SafeAreaResizerEditorUtils.GlobalTestSafeAreaRect = rect;
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
        
        [MenuItem("Ui/Resize all safe area resizers")]
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