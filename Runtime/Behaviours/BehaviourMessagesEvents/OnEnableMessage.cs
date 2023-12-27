using UnityEngine;
using UnityEngine.Events;

namespace Utils.Behaviours.BehaviourMessagesEvents
{
    public class OnEnableMessage : MonoBehaviour
    {
        [SerializeField] private UnityEvent _onEnable = new UnityEvent();

        public UnityEvent OnEnableEvent => _onEnable;

        private void OnEnable()
        {
            _onEnable.Invoke();
        }
    }
}