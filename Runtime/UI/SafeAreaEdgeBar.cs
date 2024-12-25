using UnityEngine;
using Utils.Attributes;

namespace Utils.UI
{
    public class SafeAreaEdgeBar : MonoBehaviour
    {
        [SerializeField, AutoProperty] private RectTransform _target;
        public enum Edge { Top, Bottom, Left, Right }
        [SerializeField] private Edge _edge;
        
        private void Start()
        {
            ResizeToSafeArea();
        }

        private void ResizeToSafeArea()
        {
            var rect = Screen.safeArea;

            switch (_edge)
            {
                case Edge.Top:
                    _target.anchorMin = new Vector2(rect.min.x / Screen.width, rect.max.y / Screen.height);
                    _target.anchorMax = Vector2.one;
                    break;
                case Edge.Bottom:
                    _target.anchorMin = Vector2.zero;
                    _target.anchorMax = new Vector2(rect.max.x / Screen.width, rect.min.y / Screen.height);
                    break;
                case Edge.Left:
                    _target.anchorMin = Vector2.zero;
                    _target.anchorMax = new Vector2(rect.min.x / Screen.width, 1);
                    break;
                case Edge.Right:
                    _target.anchorMin = new Vector2(rect.max.x / Screen.width, 0);
                    _target.anchorMax = Vector2.one;
                    break;
            }
        }
    }
}