using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using Utils.Editor.EditorGUIUtils;
using Object = UnityEngine.Object;

namespace Utils.Editor
{
    /// <summary>
    /// Code partially taken from https://forum.unity.com/threads/is-there-a-way-to-input-text-using-a-unity-editor-utility.473743/#post-7229248
    /// </summary>
    public class EditorInputDialog : EditorWindow
    {
        private string _description;
        private SerializedObject _target;
        private bool _initializedPosition = false;
        private Action _submitAction;
        private List<(string, Action)> _buttons;
 
        private bool _shouldClose = false;
        private Vector2 _maxScreenPos;
 
        #region OnGUI()
        void OnGUI()
        {
            // Check if Esc/Return have been pressed
            var e = Event.current;
            if(e.type == EventType.KeyDown)
            {
                switch( e.keyCode )
                {
                    // Escape pressed
                    case KeyCode.Escape:
                        _shouldClose = true;
                        e.Use();
                        break;
 
                    // Enter pressed
                    case KeyCode.Return:
                    case KeyCode.KeypadEnter:
                        _submitAction?.Invoke();
                        _shouldClose = true;
                        e.Use();
                        break;
                }
            }
 
            if(_shouldClose) {  // Close this dialog
                Close();
            }

            // Draw our control
            var rect = EditorGUILayout.BeginVertical();
 
            EditorGUILayout.Space(12);
            
            var style = new GUIStyle(EditorStyles.label)
            {
                wordWrap = true
            };
            EditorGUILayout.SelectableLabel(_description, style);
            
            EditorGUILayout.Space(8);
            GUIUtils.DrawSerializedObject(_target);
            EditorGUILayout.Space(12);
 
            // Draw OK / Cancel buttons
            var r = EditorGUILayout.GetControlRect();
            var buttonWidth = r.width / _buttons.Count;
            for (int i = 0; i < _buttons.Count; i++)
            {
                var (text, action) = _buttons[i];
                var x = r.x + i * buttonWidth;
                var buttonRect = new Rect(x, r.y, buttonWidth, r.height);
                if( GUI.Button(buttonRect, text))
                {
                    action?.Invoke();
                    _shouldClose = true;
                }
            }
 
            EditorGUILayout.Space(8);
            EditorGUILayout.EndVertical();
 
            // Force change size of the window
            if(rect.width != 0 && minSize != rect.size) {
                minSize = maxSize = rect.size;
            }
 
            // Set dialog position next to mouse position
            if(!_initializedPosition && e.type == EventType.Layout)
            {
                _initializedPosition = true;
 
                // Move window to a new position. Make sure we're inside visible window
                var mousePos = GUIUtility.GUIToScreenPoint(Event.current.mousePosition);
                mousePos.x += 32;
                if(mousePos.x + position.width > _maxScreenPos.x) mousePos.x -= position.width + 64; // Display on left side of mouse
                if(mousePos.y + position.height > _maxScreenPos.y) mousePos.y = _maxScreenPos.y - position.height;
 
                position = new Rect(mousePos.x, mousePos.y, position.width, position.height);
 
                // Focus current window
                Focus();
            }
        }
        #endregion OnGUI()
        
        #region Show()
        
        public static void Show<T>(
            string title,
            string description,
            List<(string, Action<T>)> buttons,
            Action<T> submitAction,
            bool modal = false
        ) where T : ScriptableObject
        {
            var maxPos = GUIUtility.GUIToScreenPoint(new Vector2(Screen.width, Screen.height));

            var output = CreateInstance<T>();
            var so = new SerializedObject(output);
        
            var window = CreateInstance<EditorInputDialog>();
            window._maxScreenPos = maxPos;
            window.titleContent = new GUIContent( title );
            window._description = description;
            window._target = so;
            window._submitAction = () => submitAction?.Invoke(output);
            window._buttons = buttons.ConvertAll<(string, Action)>(tuple =>
            {
                var (text, action) = tuple;
                return (text, () => action?.Invoke(output));
            });

            if (modal)
            {
                window.ShowModal();
            }
            else
            {
                window.Show();
            }
        }
        
        public static void Show<T>(
            string title,
            string description,
            List<(string, Action<T>)> buttons,
            bool modal = false
        ) where T : ScriptableObject
        {
            var submitAction = buttons.Count > 0 ? buttons[0].Item2 : null;
            Show(title, description, buttons, submitAction, modal);
        }
        
        public static void Show<T>(
            string title,
            string description,
            Action<T> onOkPressed,
            string okButton = "OK",
            string cancelButton = "Cancel",
            bool modal = false
        ) where T : ScriptableObject
        {
            var buttons = new List<(string, Action<T>)>
            {
                (okButton, onOkPressed),
                (cancelButton, null)
            };
            Show(title, description, buttons, modal);
        }
        
        public static T ShowModal<T>(
            string title,
            string description,
            string okButton = "OK",
            string cancelButton = "Cancel"
        ) where T : ScriptableObject
        {
            T output = null;
            void OkAction(T o) => output = o;
            Show(title, description, (Action<T>) OkAction, okButton, cancelButton, true);

            return output;
        }

        public static bool ShowYesNoDialog(string title, string description)
        {
            bool output = false;
            void OkAction(ScriptableObject so) => output = true;
            Show(title, description, (Action<ScriptableObject>) OkAction, "Yes", "No", true);

            return output;
        }
        
        public static void ShowMessage(
            string title,
            string message)
        {
            var buttons = new List<(string, Action<ScriptableObject>)>
            {
                ("Ok", null)
            };
            Show(title, message, buttons);
        }
        #endregion Show()
    }
}


[Serializable]
public class BoolContainer : ScriptableObject
{
    [SerializeField] private bool _value;

    public bool Value => _value;
}

[Serializable]
public class IntContainer : ScriptableObject
{
    [SerializeField] private int _value;

    public int Value => _value;
}

[Serializable]
public class FloatContainer : ScriptableObject
{
    [SerializeField] private float _value;

    public float Value => _value;
}

[Serializable]
public class StringContainer : ScriptableObject
{
    [SerializeField] private string _value = "";

    public string Value => _value;
}
