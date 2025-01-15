#if ENABLED_INPUTSYSTEM

using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;
using Utils.Input;

namespace Utils.Editor.PropertyDrawers
{
    [CustomPropertyDrawer(typeof(SchemeSelector))]
    public class ControlSchemeSelector : PropertyDrawer
    {
        private GUIContent[] _schemesLabels;
        private GUIContent[] SchemesLabels
        {
            get
            {
                if (_schemesLabels == null)
                {
                    var schemes = SchemeSelector.GetSchemes();
                    _schemesLabels = new GUIContent[schemes.Count];
                    for (int i = 0; i < schemes.Count; i++)
                    {
                        var scheme = schemes[i];
                        _schemesLabels[i] = new GUIContent(scheme.name);
                    }
                }

                return _schemesLabels;
            }
        }
            
        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            var schemeProperty = property.FindPropertyRelative("scheme");
            return EditorGUI.GetPropertyHeight(schemeProperty);
        }
        
        public override void OnGUI(Rect rect, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(rect, label, property);
            
            var schemeProperty = property.FindPropertyRelative("scheme");
            var selectedValue = (InputControlScheme) schemeProperty.boxedValue;
            var values = SchemeSelector.GetSchemes();

            int selectedValueIndex = values.IndexOf(selectedValue);
            if (selectedValueIndex < 0)
            {
                selectedValueIndex = 0;
            }
            
            Dropdown(rect, schemeProperty, selectedValue, selectedValueIndex);

            EditorGUI.EndProperty();
        }

        private void Dropdown(Rect rect, SerializedProperty property, InputControlScheme current, int selectedValueIndex)
        {
            var optionLabel = SchemesLabels[selectedValueIndex];
            var pressed = GUI.Button(rect, optionLabel, EditorStyles.popup);
            if (pressed)
            {
                EditorUtility.DisplayCustomMenu(
                    rect,
                    SchemesLabels,
                    selectedValueIndex,
                    (data, strings, selected) =>
                    {
                        var newValue = SchemeSelector.GetSchemes()[selected];
                
                        if (current != newValue)
                        {
                            property.boxedValue = newValue;
                            property.serializedObject.ApplyModifiedProperties();
                        }
                    },
                    null);
            }
        }
    }
}

#endif