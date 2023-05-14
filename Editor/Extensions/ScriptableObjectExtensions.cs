using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace Utils.Editor.Extensions
{
    public static class ScriptableObjectExtensions
    {
        public static ScriptableObject DuplicateScriptableObjectWithSubAssets(this ScriptableObject original)
        {
            // create the new path
            var originalPath = AssetDatabase.GetAssetPath(original);
            var path = AssetDatabase.GenerateUniqueAssetPath(originalPath);

            // Duplicate the original scriptable object
            ScriptableObject newScriptableObject = Object.Instantiate(original);
            AssetDatabase.CreateAsset(newScriptableObject, path);

            // Get all sub assets of the original scriptable object
            var subAssets = AssetDatabase.LoadAllAssetsAtPath(originalPath)
                .Where(subAsset => subAsset != original)
                .ToList();
            
            // Duplicate all sub assets and add them as sub assets of the new scriptable object
            Dictionary<Object, Object> originalToNewSubAssets = new Dictionary<Object, Object>();
            foreach (Object subAsset in subAssets)
            {
                Object newSubAsset = Object.Instantiate(subAsset);
                newSubAsset.name = subAsset.name;
                AssetDatabase.AddObjectToAsset(newSubAsset, newScriptableObject);
                originalToNewSubAssets.Add(subAsset, newSubAsset);
            }

            // Update all references to sub assets in the new scriptable object
            SerializedObject newSerializedObject = new SerializedObject(newScriptableObject);
            var property = newSerializedObject.GetIterator();
            while (property.NextVisible(true))
            {
                if (property.propertyType == SerializedPropertyType.ObjectReference)
                {
                    Object originalObject = property.objectReferenceValue;
                    if (originalToNewSubAssets.ContainsKey(originalObject))
                    {
                        property.objectReferenceValue = originalToNewSubAssets[originalObject];
                    }
                }
            }
            newSerializedObject.ApplyModifiedPropertiesWithoutUndo();
            EditorUtility.SetDirty(newScriptableObject);
            AssetDatabase.SaveAssets();

            return newScriptableObject;
        }
    }
}