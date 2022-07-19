using System;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Utils.Editor.EditorGUIUtils
{
    public static class GUIUtils
    {
        public static void DrawUnityObject(Object obj)
        {
            var so = new SerializedObject(obj);
            DrawSerializedObject(so);
        }

        public static void DrawSerializedObject(SerializedObject so, bool drawScript = false)
        {
            var iterator = so.GetIterator();
            bool enterChildren = true;
            while (iterator.NextVisible(enterChildren))
            {
                bool isScript = iterator.type.StartsWith("PPtr<MonoScript>");
                if (isScript && !drawScript)
                    continue;
                
                if (isScript)
                    EditorGUI.BeginDisabledGroup(true);
                enterChildren = false;
                EditorGUILayout.PropertyField(iterator, true);
                if (isScript)
                {
                    EditorGUI.EndDisabledGroup();
                    EditorGUILayout.Space(4);
                }
            }
            so.ApplyModifiedProperties();
        }
    }
}