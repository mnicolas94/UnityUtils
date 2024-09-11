﻿#if ENABLED_INPUTSYSTEM

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
        [SerializeField, Dropdown(nameof(GetSchemes))] private List<InputControlScheme> _schemes;
        [SerializeField] private UnityEvent _event;

        private DropdownList<InputControlScheme> _cachedSchemes;

        public bool MatchesScheme(InputControlScheme scheme)
        {
            var contains = _schemes.Contains(scheme);
            return contains ^ _invert;
        }

        public void Invoke()
        {
            _event.Invoke();
        }

        private DropdownList<InputControlScheme> GetSchemes()
        {
            if (_cachedSchemes == null)
            {
                _cachedSchemes = new DropdownList<InputControlScheme>();
                var actions = new DefaultInputActions();
                
                var schemes = actions.controlSchemes;
                foreach (var scheme in schemes)
                {
                    _cachedSchemes.Add(scheme.name, scheme);
                }
            }

            return _cachedSchemes;
        }
    }
}

#endif