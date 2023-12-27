using UnityEngine;
using UnityEngine.Events;

namespace Utils.Behaviours.BehaviourMessagesEvents
{
    public class StartMessage : MonoBehaviour
    {
        [SerializeField] private UnityEvent _onStart = new UnityEvent();

        public UnityEvent OnStartEvent => _onStart;

        private void Start()
        {
            _onStart.Invoke();
        }
    }
}