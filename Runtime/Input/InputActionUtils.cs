#if ENABLED_INPUTSYSTEM

using UnityEngine.InputSystem;

namespace Utils.Input
{
    public static class InputActionUtils
    {
        public static InputAction GetKeyAction(Key key)
        {
            var action = new InputAction(key.ToString());
            action.AddBinding(Keyboard.current[key]);

            return action;
        }
        
        public static InputAction GetTapAction()
        {
            var inputAction = new InputAction("Tap", InputActionType.PassThrough, interactions: "Tap");
            AddPointerBindings(inputAction);

            return inputAction;
        }
        
        public static InputAction GetDoubleTapAction()
        {
            var inputAction = new InputAction("MultiTap", InputActionType.PassThrough, interactions: "MultiTap(tapCount=2)");
            AddPointerBindings(inputAction);

            return inputAction;
        }
        
        public static InputAction GetClickAction()
        {
            var inputAction = new InputAction("Click", InputActionType.PassThrough);
            AddPointerBindings(inputAction);

            return inputAction;
        }
        
        public static InputAction GetPointAction(bool singleTouch = true, InputActionType inputActionType = InputActionType.Value)
        {
            var touchPath = singleTouch ? "<Touchscreen>/touch0/position" : "<Touchscreen>/touch*/position";
            var inputAction = new InputAction("Point", inputActionType);
            inputAction.AddBinding("<Mouse>/position", groups: "Keyboard&Mouse");
            inputAction.AddBinding("<Pen>/position", groups: "Keyboard&Mouse");
            inputAction.AddBinding(touchPath, groups: "Touch");

            return inputAction;
        }
        
        public static InputAction GetBackAction()
        {
            var inputAction = new InputAction("Back", InputActionType.Button);
            inputAction.AddBinding("<Keyboard>/escape", groups: "Keyboard&Mouse");
            inputAction.AddBinding("*/{Back}", groups: "Touch");

            return inputAction;
        }

        private static void AddPointerBindings(InputAction inputAction)
        {
            inputAction.AddBinding("<Mouse>/leftButton", groups: "Keyboard&Mouse");
            inputAction.AddBinding("<Pen>/tip", groups: "Keyboard&Mouse");
            inputAction.AddBinding("<Touchscreen>/touch*/press", groups: "Touch");
            inputAction.AddBinding("<XRController>/trigger", groups: "XR");
        }
    }
}

#endif