﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;
using Utils.Attributes;


namespace Utils.Editor.AttributeDrawers
{
    /// <summary>
    /// This code was taken from https://github.com/dbrizov/NaughtyAttributes
    /// and modified by me.
    /// </summary>
    [CustomPropertyDrawer(typeof(DropdownAttribute))]
    public class DropdownPropertyDrawer : PropertyDrawer
    {
        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return EditorGUI.GetPropertyHeight(property, includeChildren: true);
        }

        public override void OnGUI(Rect rect, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(rect, label, property);

            DropdownAttribute dropdownAttribute = (DropdownAttribute)attribute;
            object target = PropertiesUtils.GetTargetObjectWithProperty(property);

            object valuesObject = GetValues(property, dropdownAttribute.ValuesName);
            FieldInfo dropdownField = ReflectionUtility.GetField(target, property.name);

            if (AreValuesValid(valuesObject, dropdownField))
            {
                object selectedValue = dropdownField.GetValue(target);
                List<object> values = new List<object>();
                List<string> displayOptions = new List<string>();
                if (valuesObject is IList && dropdownField.FieldType == GetElementType(valuesObject))
                {
                    // Values and display options
                    IList valuesList = (IList)valuesObject;

                    for (int i = 0; i < valuesList.Count; i++)
                    {
                        object value = valuesList[i];
                        var displayOption = value == null ? "<null>" : value.ToString();
                        values.Add(value);
                        displayOptions.Add(displayOption);
                    }
                }
                else if (valuesObject is IDropdownList)
                {
                    // Current value index, values and display options
                    IDropdownList dropdown = (IDropdownList)valuesObject;

                    using (IEnumerator<KeyValuePair<string, object>> dropdownEnumerator = dropdown.GetEnumerator())
                    {
                        while (dropdownEnumerator.MoveNext())
                        {
                            KeyValuePair<string, object> current = dropdownEnumerator.Current;

                            values.Add(current.Value);

                            if (current.Key == null)
                            {
                                displayOptions.Add("<null>");
                            }
                            else if (string.IsNullOrWhiteSpace(current.Key))
                            {
                                displayOptions.Add("<empty>");
                            }
                            else
                            {
                                displayOptions.Add(current.Key);
                            }
                        }
                    }
                }
                
                int selectedValueIndex = values.IndexOf(selectedValue);
                if (selectedValueIndex < 0)
                {
                    selectedValueIndex = 0;
                }

                Dropdown(
                    rect,
                    property.serializedObject,
                    target,
                    dropdownField,
                    label.text,
                    selectedValueIndex,
                    values.ToArray(),
                    displayOptions.ToArray());
            }
            else
            {
                string message = string.Format("Invalid values with name '{0}' provided to '{1}'. Either the values name is incorrect or the types of the target field and the values field/property/method don't match",
                    dropdownAttribute.ValuesName, dropdownAttribute.GetType().Name);

                EditorGUI.PropertyField(rect, property, true);

            }

            EditorGUI.EndProperty();
        }

        private object GetValues(SerializedProperty property, string valuesName)
        {
            object target = PropertiesUtils.GetTargetObjectWithProperty(property);

            FieldInfo valuesFieldInfo = ReflectionUtility.GetField(target, valuesName);
            if (valuesFieldInfo != null)
            {
                return valuesFieldInfo.GetValue(target);
            }

            PropertyInfo valuesPropertyInfo = ReflectionUtility.GetProperty(target, valuesName);
            if (valuesPropertyInfo != null)
            {
                return valuesPropertyInfo.GetValue(target);
            }

            MethodInfo methodValuesInfo = ReflectionUtility.GetMethod(target, valuesName);
            if (methodValuesInfo != null &&
                methodValuesInfo.ReturnType != typeof(void) &&
                methodValuesInfo.GetParameters().Length == 0)
            {
                return methodValuesInfo.Invoke(target, null);
            }

            return null;
        }

        private bool AreValuesValid(object values, FieldInfo dropdownField)
        {
            if (values == null || dropdownField == null)
            {
                return false;
            }

            if ((values is IList && dropdownField.FieldType == GetElementType(values)) ||
                (values is IDropdownList))
            {
                return true;
            }

            return false;
        }

        private Type GetElementType(object values)
        {
            Type valuesType = values.GetType();
            Type elementType = ReflectionUtility.GetListElementType(valuesType);

            return elementType;
        }
        
        public static void Dropdown(
            Rect rect, SerializedObject serializedObject, object target, FieldInfo dropdownField,
            string label, int selectedValueIndex, object[] values, string[] displayOptions)
        {
            var optionLabel = new GUIContent(displayOptions[selectedValueIndex]);
            var controlRect = EditorGUI.PrefixLabel(rect, new GUIContent(label));
            var pressed = GUI.Button(controlRect, optionLabel, EditorStyles.popup);
            if (pressed)
            {
                var options = displayOptions.Select(option => new GUIContent(option)).ToArray();
                EditorUtility.DisplayCustomMenu(
                    controlRect,
                    options,
                    selectedValueIndex,
                    (data, strings, selected) =>
                    {
                        object newValue = values[selected];
            
                        object dropdownValue = dropdownField.GetValue(target);
                        if (dropdownValue == null || !dropdownValue.Equals(newValue))
                        {
                            Undo.RecordObject(serializedObject.targetObject, "Dropdown");
            
                            // TODO: Problem with structs, because they are value type.
                            // The solution is to make boxing/unboxing but unfortunately I don't know the compile time type of the target object
                            dropdownField.SetValue(target, newValue);
                            // to ensure values get stored on ScriptableObjects
                            EditorUtility.SetDirty(serializedObject.targetObject);
                        }
                    },
                    null);
            }
        }
    }
}