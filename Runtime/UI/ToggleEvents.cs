using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Utils.UI
{
    public class ToggleEvents : MonoBehaviour
    {
        [SerializeField] private Toggle _toggle;
        [SerializeField] private UnityEvent _onTrue;
        [SerializeField] private UnityEvent _onFalse;

        private void Start()
        {
            _toggle.onValueChanged.AddListener(OnValueChanged);
            OnValueChanged(_toggle.isOn);
        }

        private void OnValueChanged(bool value)
        {
            if (value)
            {
                _onTrue.Invoke();
            }
            else
            {
                _onFalse.Invoke();
            }
        }
    }
}