using UnityEngine;
using Utils.Attributes;

namespace Utils.UI
{
    public class SafeAreaRectTransform : MonoBehaviour
    {
        [SerializeField, AutoProperty] private RectTransform _target;
        
        private void Start()
        {
            TransformToSafeArea();
        }

        private void TransformToSafeArea()
        {
            var safeRect = Screen.safeArea;

            var anchorMin = new Vector2(
                safeRect.min.x + _target.anchorMin.x * safeRect.width,
                safeRect.min.y + _target.anchorMin.y * safeRect.height);
            var anchorMax = new Vector2(
                safeRect.min.x + _target.anchorMax.x * safeRect.width,
                safeRect.min.y + _target.anchorMax.y * safeRect.height);

            anchorMin.x /= Screen.width;
            anchorMin.y /= Screen.height;
            anchorMax.x /= Screen.width;
            anchorMax.y /= Screen.height;

            _target.anchorMin = anchorMin;
            _target.anchorMax = anchorMax;
        }
    }
}