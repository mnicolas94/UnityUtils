using UnityEngine;
using UnityEngine.Events;

namespace Utils.Behaviours.BehaviourMessagesEvents
{
    public class OnDisableMessage : MonoBehaviour
    {
        [SerializeField] private UnityEvent _onDisable = new UnityEvent();

        public UnityEvent OnDisableEvent => _onDisable;

        private void OnDisable()
        {
            _onDisable.Invoke();
        }
    }
}