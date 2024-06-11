using UnityEngine;
using UnityEngine.Events;

namespace Utils.Behaviours
{
    public class InvokeUnityEvent : MonoBehaviour
    {
        [SerializeField] private UnityEvent _event;

        public void Invoke()
        {
            _event.Invoke();
        }
    }
}