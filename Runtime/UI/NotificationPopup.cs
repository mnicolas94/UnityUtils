using TMPro;
using UnityEngine;

namespace Utils.UI
{
    public class NotificationPopup : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI _text;

        public void SetText(string text)
        {
            _text.text = text;
        }
    }
}