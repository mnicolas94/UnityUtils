#if ENABLED_INPUTSYSTEM

using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using Utils.Attributes;

namespace Utils.Input
{
    public class InputSchemeChange : MonoBehaviour
    {
        [SerializeField] private List<SchemeEvent> _events;
        [SerializeField] private bool _triggerOnStart;
        
        private void OnEnable()
        {
            InputSchemeObserverAsset.Instance.OnSchemeChanged += CallEvents;
            if (_triggerOnStart)
            {
                CallCurrentSchemeEvents();
            }
        }

        private void OnDisable()
        {
            InputSchemeObserverAsset.Instance.OnSchemeChanged -= CallEvents;
        }

        private void CallCurrentSchemeEvents()
        {
            CallEvents(InputSchemeObserverAsset.Instance.CurrentScheme);
        }
        
        private void CallEvents(InputControlScheme scheme)
        {
            foreach (var schemeEvent in _events)
            {
                if (schemeEvent.MatchesScheme(scheme))
                {
                    schemeEvent.Invoke();
                }
            }
        }
    }

    [Serializable]
    public class SchemeEvent
    {
        [SerializeField] private bool _invert;
        [SerializeField] private List<SchemeSelector> _schemes;
        [SerializeField] private UnityEvent _event;

        public bool MatchesScheme(InputControlScheme scheme)
        {
            var contains = _schemes.Exists(selector => selector.scheme == scheme);
            return contains ^ _invert;
        }

        public void Invoke()
        {
            _event.Invoke();
        }
    }

    [Serializable]
    public struct SchemeSelector
    {
        [SerializeField, Dropdown(nameof(GetSchemes))] public InputControlScheme scheme;
        
        static SchemeSelector()
        {
            _cachedSchemes = null;
        }
        
        private static List<InputControlScheme> _cachedSchemes;
        public static List<InputControlScheme> GetSchemes()
        {
            if (_cachedSchemes == null)
            {
                _cachedSchemes = new List<InputControlScheme>();
                var actions = new DefaultInputActions();
                
                var schemes = actions.controlSchemes;
                foreach (var scheme in schemes)
                {
                    _cachedSchemes.Add(scheme);
                }
            }

            return _cachedSchemes;
        }
    }
}

#endif