using System.IO;
using UnityEditor;
using UnityEngine;
using Utils.Attributes;

namespace Utils.Editor.AttributeDrawers
{
    [CustomPropertyDrawer(typeof(PathSelectorAttribute))]
    public class PathSelectorAttributeDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            // EditorGUI.BeginProperty(position, label, property);

            // Get the attribute data
            var pathSelectorAttribute = attribute as PathSelectorAttribute;
            bool isRelative = pathSelectorAttribute.IsRelative;
            bool isDirectory = pathSelectorAttribute.IsDirectory;

            // Draw the property label
            position = EditorGUI.PrefixLabel(position, label);

            // Draw the property field
            DrawSelectorButton(position, property, isRelative, isDirectory);

            // EditorGUI.EndProperty();
        }

        private void DrawSelectorButton(Rect position, SerializedProperty property, bool isRelative, bool isDirectory)
        {
            string path = property.stringValue;

            var textRect = new Rect(position);
            textRect.width -= position.height;
            var buttonRect = new Rect(position.x + position.width - position.height, position.y, position.height, position.height);

            // Draw the text field with the selected path
            EditorGUI.BeginChangeCheck();
            path = EditorGUI.TextField(textRect, path);
            if (EditorGUI.EndChangeCheck())
            {
                property.stringValue = path;
            }

            // Draw the button to open the path picker
            if (GUI.Button(buttonRect, EditorGUIUtility.IconContent("Folder Icon")))
            {
                string selectedPath;
                if (isDirectory)
                {
                    selectedPath = EditorUtility.OpenFolderPanel("Select Directory", path, "");
                }
                else
                {
                    var directory = string.IsNullOrEmpty(path) ? "" : Path.GetDirectoryName(path);
                    selectedPath = EditorUtility.OpenFilePanel("Select File", directory, "");
                }
                if (!string.IsNullOrEmpty(selectedPath))
                {
                    if (isRelative)
                    {
                        selectedPath = Path.GetRelativePath("./", selectedPath);
                    }
                    property.stringValue = selectedPath;
                    property.serializedObject.ApplyModifiedProperties();
                }
                GUIUtility.ExitGUI();
            }
        }
    }
}