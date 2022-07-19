using System;
using UnityEditor;
using UnityEngine;
using Utils.Editor.EditorGUIUtils;

namespace Utils.Editor
{
    /// <summary>
    /// Code partially taken from https://forum.unity.com/threads/is-there-a-way-to-input-text-using-a-unity-editor-utility.473743/#post-7229248
    /// </summary>
    public class EditorInputDialogScriptableObject : EditorWindow
    {
        private string _description;
        private SerializedObject _target;
        private string _okButton;
        private string _cancelButton;
        private bool _initializedPosition = false;
        private Action _onOkButton;
 
        private bool _shouldClose = false;
        private Vector2 _maxScreenPos;
 
        #region OnGUI()
        void OnGUI()
        {
            // Check if Esc/Return have been pressed
            var e = Event.current;
            if( e.type == EventType.KeyDown )
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
                        _onOkButton?.Invoke();
                        _shouldClose = true;
                        e.Use();
                        break;
                }
            }
 
            if( _shouldClose ) {  // Close this dialog
                Close();
                //return;
            }

            // Draw our control
            var rect = EditorGUILayout.BeginVertical();
 
            EditorGUILayout.Space( 12 );
            EditorGUILayout.LabelField( _description );

            EditorGUILayout.Space( 8 );
            GUIUtils.DrawSerializedObject(_target);
            EditorGUILayout.Space( 12 );
 
            // Draw OK / Cancel buttons
            var r = EditorGUILayout.GetControlRect();
            r.width /= 2;
            if( GUI.Button( r, _okButton ) ) {
                _onOkButton?.Invoke();
                _shouldClose = true;
            }
 
            r.x += r.width;
            if( GUI.Button( r, _cancelButton ) ) {
//            _inputText = null;   // Cancel - delete inputText
                _shouldClose = true;
            }
 
            EditorGUILayout.Space( 8 );
            EditorGUILayout.EndVertical();
 
            // Force change size of the window
            if( rect.width != 0 && minSize != rect.size ) {
                minSize = maxSize = rect.size;
            }
 
            // Set dialog position next to mouse position
            if( !_initializedPosition && e.type == EventType.Layout )
            {
                _initializedPosition = true;
 
                // Move window to a new position. Make sure we're inside visible window
                var mousePos = GUIUtility.GUIToScreenPoint( Event.current.mousePosition );
                mousePos.x += 32;
                if( mousePos.x + position.width > _maxScreenPos.x ) mousePos.x -= position.width + 64; // Display on left side of mouse
                if( mousePos.y + position.height > _maxScreenPos.y ) mousePos.y = _maxScreenPos.y - position.height;
 
                position = new Rect( mousePos.x, mousePos.y, position.width, position.height );
 
                // Focus current window
                Focus();
            }
        }
        #endregion OnGUI()
 
        #region Show()
    
        public static bool Show<T>(
            string title,
            string description,
            out T output,
            string okButton = "OK",
            string cancelButton = "Cancel")
            where T : ScriptableObject
        {
            var maxPos = GUIUtility.GUIToScreenPoint( new Vector2( Screen.width, Screen.height ) );

            bool success = false;
            output = CreateInstance<T>();
            var so = new SerializedObject(output);
        
            var window = CreateInstance<EditorInputDialogScriptableObject>();
            window._maxScreenPos = maxPos;
            window.titleContent = new GUIContent( title );
            window._description = description;
            window._target = so;
            window._okButton = okButton;
            window._cancelButton = cancelButton;
            window._onOkButton = () => success = true;
            window.ShowModal();
 
            return success;
        }
        #endregion Show()
    }
}