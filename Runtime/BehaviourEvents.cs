using System;
using UnityEngine;
using UnityEngine.Events;

namespace Utils
{
    public class BehaviourEvents : MonoBehaviour
    {
        [SerializeField] private UnityEvent _onAwake;
        [SerializeField] private UnityEvent _onStart;
        [SerializeField] private UnityEvent _onEnable;
        [SerializeField] private UnityEvent _onDisable;
        [SerializeField] private UnityEvent _onDestroy;

        public UnityEvent OnAwakeEvent => _onAwake;

        public UnityEvent OnStartEvent => _onStart;

        public UnityEvent OnEnableEvent => _onEnable;

        public UnityEvent OnDisableEvent => _onDisable;

        public UnityEvent OnDestroyEvent => _onDestroy;

        private void Awake()
        {
            _onAwake?.Invoke();
        }

        private void Start()
        {
            _onStart?.Invoke();
        }

        private void OnEnable()
        {
            _onEnable?.Invoke();
        }

        private void OnDisable()
        {
            _onDisable?.Invoke();
        }

        private void OnDestroy()
        {
            _onDestroy?.Invoke();
        }
    }
}