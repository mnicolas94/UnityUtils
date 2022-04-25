using System;
using System.Reflection;
using UnityEngine;
using UnityEditor;

namespace Utils.Editor
{
    /**
     * Inspiration and some methods were taken from
     * https://github.com/lordofduct/spacepuppy-unity-framework-4.0/blob/master/Framework/com.spacepuppy.core/
     */
    [CustomPropertyDrawer(typeof(TypeReference), true)]
    public class TypeReferencePropertyDrawer : PropertyDrawer
    {
        public const string PROP_TYPEHASH = "_typeHash";
        
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(position, label, property);

            if (PropertiesUtils.GetTargetObjectOfProperty(property) is TypeReference target)
            {
                var baseType = target.GetBaseType();
                var tp = GetTypeFromTypeReference(property, baseType);
                
                var rect = EditorGUI.PrefixLabel(position, label);
                TypeDropDown(rect, label, property, baseType, tp);
            }
            
            EditorGUI.EndProperty();
        }

        private void TypeDropDown(Rect position, GUIContent label, SerializedProperty property, Type baseType, Type currentType)
        {
            string text = currentType == null ? "Null" : currentType.Name;
            bool pressed = EditorGUI.DropdownButton(position, new GUIContent(text), FocusType.Keyboard);
            if (pressed)
            {
                var childTypes = TypeUtil.GetSubclassTypes(baseType);
                var typesNames = childTypes.ConvertAll(tp => tp.Name).ToArray();
                
                var menu = new GenericMenu();
                for (int i = 0; i < typesNames.Length; i++)
                {
                    string typeName = typesNames[i];
                    menu.AddItem(
                        new GUIContent(typeName),
                        typeName == text,
                        index =>
                        {
                            var type = childTypes[(int) index];
                            SetTypeToTypeReference(property, type);
                        },
                        i
                        );
                }
                menu.ShowAsContext();
            }
        }
        
        private Type GetTypeFromTypeReference(SerializedProperty property, Type baseType)
        {
            var hashProp = property?.FindPropertyRelative(PROP_TYPEHASH);
            if (hashProp == null) return null;
            return TypeReference.UnHashType(hashProp.stringValue, baseType);
        }
        
        private void SetTypeToTypeReference(SerializedProperty property, Type tp)
        {
            var hashProp = property?.FindPropertyRelative(PROP_TYPEHASH);
            if (hashProp == null) return;
            hashProp.stringValue = TypeReference.HashType(tp);
            property.serializedObject.ApplyModifiedProperties();
        }
    }
}