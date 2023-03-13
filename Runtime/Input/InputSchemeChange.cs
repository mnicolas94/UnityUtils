using System;
using System.Collections.Generic;
using System.Linq;
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
        
        private DefaultInputActions _actions;
        private DefaultInputActions Actions => _actions ??= new DefaultInputActions();
        private InputControlScheme _lastScheme;
        
        private void Start()
        {
            var mapUi = Actions.asset.FindActionMap("UI");
            var mapPlayer = Actions.asset.FindActionMap("Player");
            mapUi.Enable();
            mapPlayer.Enable();
            mapUi.actionTriggered += OnActionTriggered;
            mapPlayer.actionTriggered += OnActionTriggered;

            if (_triggerOnStart)
            {
                var device = Actions.asset.actionMaps[0].actions[0].activeControl.device;
                var scheme = Actions.controlSchemes.First(scheme => scheme.SupportsDevice(device));
                Debug.Log(scheme);
                NotifyScheme(scheme);
            }
        }

        private void OnEnable()
        {
            Actions.Enable();
        }

        private void OnDisable()
        {
            Actions.Disable();
        }

        private void OnActionTriggered(InputAction.CallbackContext ctx)
        {
            var device = ctx.control.device;
            var scheme = Actions.controlSchemes.First(scheme => scheme.SupportsDevice(device));

            if (_lastScheme == scheme)
                return;

            NotifyScheme(scheme);
        }

        private void NotifyScheme(InputControlScheme scheme)
        {
            _lastScheme = scheme;
            CallEvents(scheme);
        }

        private void CallEvents(InputControlScheme scheme)
        {
            foreach (var schemeEvent in _events)
            {
                if (schemeEvent.Scheme == scheme)
                {
                    schemeEvent.Event.Invoke();
                }
            }
        }
    }

    [Serializable]
    public class SchemeEvent
    {
        [SerializeField, Dropdown(nameof(GetSchemes))] private InputControlScheme _scheme;
        [SerializeField] private UnityEvent _event;

        private DropdownList<InputControlScheme> _cachedSchemes;

        public InputControlScheme Scheme => _scheme;

        public UnityEvent Event => _event;

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