﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using UnityEditor;
using Object = UnityEngine.Object;

namespace Utils.Editor
{
    /// <summary>
    /// This code was taken from https://github.com/dbrizov/NaughtyAttributes
    /// </summary>
    public static class PropertiesUtils
    {
        public static IEnumerable<SerializedProperty> GetSerializedProperties(
            SerializedObject so, bool returnScript=false , bool recursive = false)
        {
            var iterator = so.GetIterator();
            bool enterChildren = true;
            while (iterator.NextVisible(enterChildren))
            {
                var sp = iterator.Copy();
                bool isScript = sp.type.StartsWith("PPtr<MonoScript>");
                if (!isScript || returnScript)
                {
                    yield return sp;
                }
                
                enterChildren = recursive;
            }
        }
        
        public static IEnumerable<SerializedProperty> GetSerializedProperties(
            SerializedProperty originalSp, bool returnScript=false , bool recursive = false)
        {
            var iterator = originalSp.Copy();
            var originalDepth = iterator.depth;
            bool enterChildren = true;
            while (iterator.NextVisible(enterChildren))
            {
                var isOutOfOriginalParent = iterator.depth <= originalDepth;
                if (isOutOfOriginalParent)
                {
                    break;
                }
                
                var sp = iterator.Copy();
                bool isScript = sp.type.StartsWith("PPtr<MonoScript>");
                if (!isScript || returnScript)
                {
                    yield return sp;
                }
                
                enterChildren = recursive;
            }
        }
        
        public static SerializedProperty FindParentProperty(this SerializedProperty serializedProperty)
        {
            var propertyPaths = serializedProperty.propertyPath.Split('.');
            if (propertyPaths.Length <= 1)
            {
                return default;
            }

            var parentSerializedProperty = serializedProperty.serializedObject.FindProperty(propertyPaths.First());
            for (var index = 1; index < propertyPaths.Length - 1; index++)
            {
                if (propertyPaths[index] == "Array" && propertyPaths.Length > index + 1 && Regex.IsMatch(propertyPaths[index + 1], "^data\\[\\d+\\]$"))
                {
                    var match = Regex.Match(propertyPaths[index + 1], "^data\\[(\\d+)\\]$");
                    var arrayIndex = int.Parse(match.Groups[1].Value);
                    parentSerializedProperty = parentSerializedProperty.GetArrayElementAtIndex(arrayIndex);
                    index++;
                }
                else
                {
                    parentSerializedProperty = parentSerializedProperty.FindPropertyRelative(propertyPaths[index]);
                }
            }

            return parentSerializedProperty;
        }

        public static T GetFieldByName<T>(SerializedProperty property, string fieldName)
        {
            object target = GetTargetObjectWithProperty(property);

            FieldInfo fieldInfo = ReflectionUtility.GetField(target, fieldName);
            if (fieldInfo != null &&
                fieldInfo.FieldType == typeof(T))
            {
                var field = (T) fieldInfo.GetValue(target);
                return field;
            }

            PropertyInfo propertyInfo = ReflectionUtility.GetProperty(target, fieldName);
            if (propertyInfo != null &&
                propertyInfo.PropertyType == typeof(T))
            {
                T field = (T) propertyInfo.GetValue(target);
                return field;
            }

            MethodInfo getterMethodInfo = ReflectionUtility.GetMethod(target, fieldName);
            if (getterMethodInfo != null &&
                getterMethodInfo.ReturnType == typeof(T) &&
                getterMethodInfo.GetParameters().Length == 0)
            {
                T field = (T) getterMethodInfo.Invoke(target, null);
                return field;
            }

            return default;
        }

        public static MemberInfo GetMemberInfoByName(SerializedProperty property)
        {
            var fieldName = property.name;
            var target = GetTargetObjectWithProperty(property);
            FieldInfo fieldInfo = ReflectionUtility.GetField(target, fieldName);
            if (fieldInfo != null)
            {
                return fieldInfo;
            }

            PropertyInfo propertyInfo = ReflectionUtility.GetProperty(target, fieldName);
            if (propertyInfo != null)
            {
                return propertyInfo;
            }

            MethodInfo getterMethodInfo = ReflectionUtility.GetMethod(target, fieldName);
            if (getterMethodInfo != null)
            {
                return getterMethodInfo;
            }

            return null;
        }
        
        /// <summary>
        /// Gets the object that the property is a member of
        /// </summary>
        /// <param name="property"></param>
        /// <returns></returns>
        public static object GetTargetObjectWithProperty(SerializedProperty property)
        {
            string path = property.propertyPath.Replace(".Array.data[", "[");
            object obj = property.serializedObject.targetObject;
            string[] elements = path.Split('.');

            for (int i = 0; i < elements.Length - 1; i++)
            {
                string element = elements[i];
                if (element.Contains("["))
                {
                    string elementName = element.Substring(0, element.IndexOf("["));
                    int index = Convert.ToInt32(element.Substring(element.IndexOf("[")).Replace("[", "").Replace("]", ""));
                    obj = GetValue_Imp(obj, elementName, index);
                }
                else
                {
                    obj = GetValue_Imp(obj, element);
                }
            }

            return obj;
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
            {
                return null;
            }

            Type type = source.GetType();

            while (type != null)
            {
                FieldInfo field = type.GetField(name, BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance);
                if (field != null)
                {
                    return field.GetValue(source);
                }

                PropertyInfo property = type.GetProperty(name, BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance | BindingFlags.IgnoreCase);
                if (property != null)
                {
                    return property.GetValue(source, null);
                }

                type = type.BaseType;
            }

            return null;
        }

        private static object GetValue_Imp(object source, string name, int index)
        {
            IEnumerable enumerable = GetValue_Imp(source, name) as IEnumerable;
            if (enumerable == null)
            {
                return null;
            }

            IEnumerator enumerator = enumerable.GetEnumerator();
            for (int i = 0; i <= index; i++)
            {
                if (!enumerator.MoveNext())
                {
                    return null;
                }
            }

            return enumerator.Current;
        }
    }
}