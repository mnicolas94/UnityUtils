using UnityEditor;
using UnityEngine;
using Utils.Attributes;

namespace Utils.Editor.AttributeDrawers
{
    [CustomPropertyDrawer(typeof(ToStringLabelAttribute))]
    public class ToStringLabelAttributeDrawer : PropertyDrawer
    {
        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return EditorGUI.GetPropertyHeight(property, label);
        }
        
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            try
            {
                var target = PropertiesUtils.GetTargetObjectOfProperty(property);
                string description = target.ToString();
                EditorGUI.PropertyField(position, property, new GUIContent(description), true);
            }
            catch
            {
                EditorGUI.PropertyField(position, property, label, true);
            }
        }
    }
}