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

            if (GetTargetObjectOfProperty(property) is TypeReference target)
            {
                var baseType = target.GetBaseType();
                var tp = target.Type;
                
                var rect = EditorGUI.PrefixLabel(position, label);
                TypeDropDown(rect, label, target, baseType, tp);
            }
            
            EditorGUI.EndProperty();
        }

        private void TypeDropDown(Rect position, GUIContent label, TypeReference property, Type baseType, Type currentType)
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
                            property.Type = type;
                        },
                        i
                        );
                }
                menu.ShowAsContext();
            }
        }
        
        private void SetTypeToTypeReference(SerializedProperty property, Type tp)
        {
            var hashProp = property?.FindPropertyRelative(PROP_TYPEHASH);
            if (hashProp == null) return;
            hashProp.stringValue = TypeReference.HashType(tp);
        }
        
        public static object GetTargetObjectOfProperty(SerializedProperty prop)
        {
            if (prop == null) return null;

            var path = prop.propertyPath.Replace(".Array.data[", "[");
            object obj = prop.serializedObject.targetObject;
            var elements = path.Split('.');
            foreach (var element in elements)
            {
                if (element.Contains("["))
                {
                    var elementName = element.Substring(0, element.IndexOf("["));
                    var index = Convert.ToInt32(element.Substring(element.IndexOf("[")).Replace("[", "").Replace("]", ""));
                    obj = GetValue_Imp(obj, elementName, index);
                }
                else
                {
                    obj = GetValue_Imp(obj, element);
                }
            }
            return obj;
        }
        
        private static object GetValue_Imp(object source, string name)
        {
            if (source == null)
                return null;
            var type = source.GetType();

            while (type != null)
            {
                var f = type.GetField(name, BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance);
                if (f != null)
                    return f.GetValue(source);

                var p = type.GetProperty(name, BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance | BindingFlags.IgnoreCase);
                if (p != null)
                    return p.GetValue(source, null);

                type = type.BaseType;
            }
            return null;
        }

        private static object GetValue_Imp(object source, string name, int index)
        {
            var enumerable = GetValue_Imp(source, name) as System.Collections.IEnumerable;
            if (enumerable == null) return null;
            var enm = enumerable.GetEnumerator();

            for (int i = 0; i <= index; i++)
            {
                if (!enm.MoveNext()) return null;
            }
            return enm.Current;
        }
    }
}