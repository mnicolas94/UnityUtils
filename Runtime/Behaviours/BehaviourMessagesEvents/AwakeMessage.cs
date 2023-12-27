using UnityEngine;
using UnityEngine.Events;

namespace Utils.Behaviours.BehaviourMessagesEvents
{
    public class AwakeMessage : MonoBehaviour
    {
        [SerializeField] private UnityEvent _onAwake = new UnityEvent();

        public UnityEvent OnAwakeEvent => _onAwake;

        private void Awake()
        {
            _onAwake.Invoke();
        }
    }
}