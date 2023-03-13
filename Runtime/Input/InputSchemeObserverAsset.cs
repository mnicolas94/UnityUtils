using System;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Utils.Input
{
    [CreateAssetMenu(fileName = "InputSchemeObserverAsset", menuName = "Facticus/Utils/InputSchemeObserverAsset")]
    public class InputSchemeObserverAsset : ScriptableObjectSingleton<InputSchemeObserverAsset>
    {
        private DefaultInputActions _actions;
        private DefaultInputActions Actions => _actions ??= new DefaultInputActions();
        
        private InputControlScheme _currentScheme;
        public InputControlScheme CurrentScheme => _currentScheme;

        public Action<InputControlScheme> OnSchemeChanged;

        protected override void OnEnableCallback()
        {
            Actions.Enable();
            foreach (var map in Actions.asset.actionMaps)
            {
                map.actionTriggered += OnActionTriggered;
            }
        }

        private void OnActionTriggered(InputAction.CallbackContext ctx)
        {
            var device = ctx.control.device;
            var scheme = Actions.controlSchemes.First(scheme => scheme.SupportsDevice(device));

            if (_currentScheme == scheme)
                return;

            _currentScheme = scheme;
            OnSchemeChanged?.Invoke(scheme);
        }
    }
}