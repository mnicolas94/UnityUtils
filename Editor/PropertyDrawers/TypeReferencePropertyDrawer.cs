using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEngine;
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

    public static class FixTypeHashes
    {
        [MenuItem("Tools/Facticus/Fix TypeReferences hashes")]
        public static void FixNotPreview()
        {
            Fix(false);
        }
        
        [MenuItem("Tools/Facticus/Fix TypeReferences hashes preview")]
        public static void FixPreview()
        {
            Fix(true);
        }
        
        
        public static void Fix(bool preview=true)
        {
            var guidsList = new List<string>();
            guidsList.AddRange(AssetDatabase.FindAssets("t:ScriptableObject"));
            guidsList.AddRange(AssetDatabase.FindAssets("t:Scene"));
            guidsList.AddRange(AssetDatabase.FindAssets("t:Prefab"));
            
            var assetsPaths = guidsList.ConvertAll(AssetDatabase.GUIDToAssetPath);
            var changes = new List<string>();

            foreach (var assetPath in assetsPaths)
            {
                var oldLines = File.ReadAllLines(assetPath);
                var newLines = new string[oldLines.Length];
                bool anyChange = false;
                for (int i = 0; i < oldLines.Length; i++)
                {
                    string line = oldLines[i];
                    string newLine = line;
                    var rx = new Regex(@"_typeHash: (?<hash>([^|]*\|)(.+))");
                    bool isReferenceLine = rx.IsMatch(line);
                    if (isReferenceLine)
                    {
                        anyChange = true;
                        
                        string oldHash = rx.Matches(line)[0].Groups["hash"].Value;
                        var type = TypeReference.OldUnHashType(oldHash);
                        string newHash = TypeReference.HashType(type);
                        newLine = rx.Replace(line, $"_typeHash: '{newHash}'");

                        var changeString = $"-Line: {$"{i + 1}".PadRight(6)} File: {assetPath}" +
                                           $"\nold line: {line}" +
                                           $"\nnew line: {newLine}";
                        changes.Add(changeString);
                    }

                    newLines[i] = newLine;
                }

                if (anyChange && !preview)
                {
                    File.WriteAllLines(assetPath, newLines);
                }
            }
            
            if (changes.Count > 0)
                Debug.Log($"Changes in files:\n{string.Join("\n\n", changes)}");
        }
    }
}