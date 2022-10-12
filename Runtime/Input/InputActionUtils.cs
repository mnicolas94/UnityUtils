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
        
        public static InputAction GetPointAction()
        {
            var inputAction = new InputAction("Point", InputActionType.Value);
            inputAction.AddBinding("<Mouse>/position", groups: "Keyboard&Mouse");
            inputAction.AddBinding("<Pen>/position", groups: "Keyboard&Mouse");
            inputAction.AddBinding("<Touchscreen>/touch*/position", groups: "Touch");

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