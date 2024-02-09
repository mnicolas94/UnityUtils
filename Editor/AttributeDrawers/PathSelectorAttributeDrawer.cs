using System;
using System.IO;
using UnityEditor;
using UnityEngine;
using Utils.Attributes;

namespace Utils.Editor.AttributeDrawers
{
    [CustomPropertyDrawer(typeof(PathSelectorAttribute))]
    public class PathSelectorAttributeDrawer : PropertyDrawer
    {
        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            var isValid = IsValidType(property);
            var height = EditorGUI.GetPropertyHeight(property, label);
            if (!isValid)
            {
                height *= 2;
            }
            return height;
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(position, label, property);

            // Get the attribute data
            var pathSelectorAttribute = attribute as PathSelectorAttribute;
            bool isRelative = pathSelectorAttribute.IsRelative;
            bool isDirectory = pathSelectorAttribute.IsDirectory;

            // Draw the property label
            position = EditorGUI.PrefixLabel(position, label);

            // validate property type
            var isValid = IsValidType(property);

            if (isValid)
            {
                // Draw the property field
                DrawSelectorButton(position, property, isRelative, isDirectory);
            }
            else
            {
                // draw warning box
                EditorGUI.HelpBox(position, $"Invalid property type. Property {property.name} should be " +
                                            $"of type string or DefaultAsset", MessageType.Error);
            }
            

            EditorGUI.EndProperty();
        }
        

        private bool IsValidType(SerializedProperty property)
        {
            var typeName = property.type;
            if (typeName is "string" or "PPtr<$DefaultAsset>")
            {
                return true;
            }
            
            return false;
        }
        
        private void DrawSelectorButton(Rect position, SerializedProperty property, bool isRelative, bool isDirectory)
        {
            // modify property rect to give space to path selection button
            var propertyRect = new Rect(position);
            propertyRect.width -= position.height;
            var buttonRect = new Rect(position.x + position.width - position.height, position.y, position.height, position.height);

            // Draw the property field
            EditorGUI.PropertyField(propertyRect, property, GUIContent.none);

            // Draw the button to open the path picker
            if (GUI.Button(buttonRect, EditorGUIUtility.IconContent("Folder Icon")))
            {
                string selectedPath;
                string currentPath = GetCurrentPath(property);

                if (isDirectory || !IsPropertyString(property))
                {
                    selectedPath = EditorUtility.OpenFolderPanel("Select Directory", currentPath, "");
                }
                else
                {
                    var directory = string.IsNullOrEmpty(currentPath) ? "" : Path.GetDirectoryName(currentPath);
                    selectedPath = EditorUtility.OpenFilePanel("Select File", directory, "");
                }
                
                if (!string.IsNullOrEmpty(selectedPath))
                {
                    if (isRelative || !IsPropertyString(property))
                    {
                        selectedPath = Path.GetRelativePath("./", selectedPath);
                    }
                    SetCurrentPath(property, selectedPath);
                    property.serializedObject.ApplyModifiedProperties();
                }
                // GUIUtility.ExitGUI();
            }
        }

        private static bool IsPropertyString(SerializedProperty property)
        {
            return property.type == "string";
        }

        private static string GetCurrentPath(SerializedProperty property)
        {
            if (IsPropertyString(property))
            {
                return property.stringValue;
            }
            else
            {
                var defaultAsset = property.objectReferenceValue as DefaultAsset;
                return AssetDatabase.GetAssetPath(defaultAsset);
            }
        }

        private void SetCurrentPath(SerializedProperty property, string path)
        {
            if (IsPropertyString(property))
            {
                property.stringValue = path;
            }
            else
            {
                var defaultAsset = AssetDatabase.LoadAssetAtPath<DefaultAsset>(path);
                property.objectReferenceValue = defaultAsset;
            }
        }
    }
}