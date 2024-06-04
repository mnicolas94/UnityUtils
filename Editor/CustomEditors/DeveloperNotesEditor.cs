using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using Utils.Behaviours;

namespace Utils.Editor.CustomEditors
{
    [CustomEditor(typeof(DeveloperNotes))]
    public class DeveloperNotesEditor : UnityEditor.Editor
    {
        public override VisualElement CreateInspectorGUI()
        {
            var notesProperty = serializedObject.FindProperty("_notes");
            var textField = new TextField();
            float lineHeight = 18f;
            float minHeight = lineHeight + (2 * 15);
            float maxHeight = lineHeight + (20 * 15);
            textField.multiline = true;
            textField.style.flexDirection = FlexDirection.Column;
            textField.style.whiteSpace = WhiteSpace.Normal;
            textField.style.minHeight = minHeight;
            textField.style.maxHeight = maxHeight;
#if UNITY_2022_1_OR_NEWER
            textField.SetVerticalScrollerVisibility(ScrollerVisibility.Auto);
#endif
            textField.bindingPath = notesProperty.propertyPath;
            return textField;
        }
    }
}