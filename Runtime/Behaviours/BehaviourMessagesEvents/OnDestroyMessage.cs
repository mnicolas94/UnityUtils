using UnityEngine;
using UnityEngine.Events;

namespace Utils.Behaviours.BehaviourMessagesEvents
{
    public class OnDestroyMessage : MonoBehaviour
    {
        [SerializeField] private UnityEvent _onDestroy = new UnityEvent();

        public UnityEvent OnDestroyEvent => _onDestroy;

        private void OnDestroy()
        {
            _onDestroy.Invoke();
        }
    }
}