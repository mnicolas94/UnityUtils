using UnityEngine;
using UnityEngine.UI;

namespace Utils.UI
{
    public class ImageAlphaHit : MonoBehaviour
    {
        [SerializeField] [Range(0.0f, 1.0f)] private float alphaThreshold;
        private Image _image;
    
        private void Start()
        {
            _image = GetComponent<Image>();
            UpdateThreshold();
        }

        [ContextMenu("Update threshold")]
        public void UpdateThreshold()
        {
            _image.alphaHitTestMinimumThreshold = alphaThreshold;
        }
    }
}