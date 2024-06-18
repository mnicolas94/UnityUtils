using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEngine;
using Utils.Editor.GenericSearchWindow;
using Utils.Serializables;

namespace Utils.Editor.PropertyDrawers
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
                var childTypes = TypeUtil.GetSubclassTypes(baseType, allowInterfaces: true, allowAbstract: true);

                var searchEntries = childTypes.ConvertAll(tp => new SearchEntry<Type>(tp.Name, tp));
                var mousePositionScreenSpace = GUIUtility.GUIToScreenPoint(position.position);
                GenericSearchWindow<Type>.Create(
                    mousePositionScreenSpace,
                    "Types",
                    searchEntries,
                    type => SetTypeToTypeReference(property, type));
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
            if (property == null)
            {
                return;
            }
            
            var hashProperty = property.FindPropertyRelative(PROP_TYPEHASH);
            hashProperty.stringValue = TypeReference.HashType(tp);
            property.serializedObject.ApplyModifiedProperties();
        }
    }
}