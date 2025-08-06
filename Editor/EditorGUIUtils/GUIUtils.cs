using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;
using Object = UnityEngine.Object;

namespace Utils.Editor.EditorGUIUtils
{
    public static class GUIUtils
    {
        private static readonly Dictionary<string, Func<SerializedProperty, VisualElement>> EmptyDrawerAdapter = new();

        public static float DrawUnityObject(Object obj, bool drawScript = false)
        {
            var so = new SerializedObject(obj);
            return DrawSerializedObject(so, drawScript);
        }

        public static float DrawSerializedObject(SerializedObject so, bool drawScript = false)
        {
            var totalHeight = 0f;
            var iterator = so.GetIterator();
            bool enterChildren = true;
            while (iterator.NextVisible(enterChildren))
            {
                bool isScript = iterator.type.StartsWith("PPtr<MonoScript>");
                if (isScript && !drawScript)
                {
                    continue;
                }
                
                if (isScript)
                    EditorGUI.BeginDisabledGroup(true);
                enterChildren = false;
                var propertyHeight = EditorGUI.GetPropertyHeight(iterator);
                totalHeight += propertyHeight;
                EditorGUILayout.PropertyField(iterator, true);
                if (isScript)
                {
                    EditorGUI.EndDisabledGroup();
                    EditorGUILayout.Space(4);
                }
            }
            so.ApplyModifiedProperties();

            return totalHeight;
        }

        public static void DrawSerializedProperties(VisualElement container, IEnumerable<SerializedProperty> properties)
        {
            DrawSerializedProperties(container, properties, EmptyDrawerAdapter);
        }
        
        public static void DrawSerializedProperties(VisualElement container, IEnumerable<SerializedProperty> properties,
            Dictionary<string, Func<SerializedProperty, VisualElement>> propertyDrawerAdapters)
        {
            foreach (var serializedProperty in properties)
            {
                VisualElement propertyField;
                if (propertyDrawerAdapters.TryGetValue(serializedProperty.name, out var propertyDrawerFactory))
                {
                    propertyField = propertyDrawerFactory(serializedProperty);
                }
                else
                {
                    propertyField = new PropertyField(serializedProperty);
                    propertyField.AddToClassList("unity-base-field__aligned");  // make widths aligned with other fields in inspector
                    propertyField.Bind(serializedProperty.serializedObject);  // doesn't work without binding manually in PropertyDrawers
                }
                container.Add(propertyField);
            }
        }
        
        public static float GetHelpBoxHeight()
        {
            return EditorGUIUtility.singleLineHeight * 2.0f;
        }

        public static float GetIndentLength(Rect sourceRect)
        {
            Rect indentRect = EditorGUI.IndentedRect(sourceRect);
            float indentLength = indentRect.x - sourceRect.x;

            return indentLength;
        }
        
        public static void DrawDefaultPropertyAndHelpBox(Rect rect, SerializedProperty property, string message)
        {
            float indentLength = GetIndentLength(rect);
            Rect helpBoxRect = new Rect(
                rect.x + indentLength,
                rect.y,
                rect.width - indentLength,
                GetHelpBoxHeight());

            EditorGUI.HelpBox(helpBoxRect, message, MessageType.Warning);

            Rect propertyRect = new Rect(
                rect.x,
                rect.y + GetHelpBoxHeight(),
                rect.width,
                EditorGUI.GetPropertyHeight(property, true));

            EditorGUI.PropertyField(propertyRect, property, true);
        }
    }
}