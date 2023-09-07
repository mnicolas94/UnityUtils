using System;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Utils.Editor.EditorGUIUtils
{
    public static class GUIUtils
    {
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