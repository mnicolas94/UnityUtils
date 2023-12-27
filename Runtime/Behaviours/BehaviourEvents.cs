using System;
using UnityEngine;
using UnityEngine.Events;

namespace Utils.Behaviours
{
    [Obsolete("Use XXXMessage components for each MonoBehaviour message. It will be better organized in editor.")]
    public class BehaviourEvents : MonoBehaviour
    {
        [SerializeField] private UnityEvent _onAwake = new UnityEvent();
        [SerializeField] private UnityEvent _onStart = new UnityEvent();
        [SerializeField] private UnityEvent _onEnable = new UnityEvent();
        [SerializeField] private UnityEvent _onDisable = new UnityEvent();
        [SerializeField] private UnityEvent _onDestroy = new UnityEvent();

        public UnityEvent OnAwakeEvent => _onAwake;

        public UnityEvent OnStartEvent => _onStart;

        public UnityEvent OnEnableEvent => _onEnable;

        public UnityEvent OnDisableEvent => _onDisable;

        public UnityEvent OnDestroyEvent => _onDestroy;

        private void Awake()
        {
            _onAwake.Invoke();
        }

        private void Start()
        {
            _onStart.Invoke();
        }

        private void OnEnable()
        {
            _onEnable.Invoke();
        }

        private void OnDisable()
        {
            _onDisable.Invoke();
        }

        private void OnDestroy()
        {
            _onDestroy.Invoke();
        }
    }
}