using UnityEditor;
using UnityEngine;

namespace Utils.Editor.PropertyDrawers
{
    [CustomPropertyDrawer(typeof(GlobalObjectIdRuntime))]
    public class GlobalObjectIdRuntimeDrawer : PropertyDrawer
    {
        private static readonly GUIContent MultipleNotSupportedLabel = new GUIContent("Multiple edit unsupported");

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            if (property.serializedObject.targetObjects.Length > 1)
            {
                EditorGUI.LabelField(position, MultipleNotSupportedLabel);
                return;
            }
            
            var idProperty = property.FindPropertyRelative("_id");
            
            EditorGUI.BeginDisabledGroup(true);
            EditorGUI.PropertyField(position, idProperty, label);
            EditorGUI.EndDisabledGroup();
            
            var id = GlobalObjectId.GetGlobalObjectIdSlow(property.serializedObject.targetObject);
            EditorGUI.BeginChangeCheck();
            idProperty.stringValue = id.ToString();
            if (EditorGUI.EndChangeCheck())
            {
                property.serializedObject.ApplyModifiedPropertiesWithoutUndo();
            }
        }

    }
}