using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using Editor;
using UnityEditor;
using UnityEngine;

namespace Utils.Editor.AutocompleteAttribute
{
    using CacheType = Dictionary<string, Dictionary<ObjectProperty, string>>;
    
    [CustomPropertyDrawer(typeof(Attributes.AutocompleteAttribute))]
    public class AutocompleteAttributePropertyDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            // assert is type string 
            if (property.propertyType != SerializedPropertyType.String)
            {
                EditorGUI.HelpBox(position, $"{property.name} is not a string but has [Autocomplete].", MessageType.Error);
            }
            else
            {
                var cache = GetCache();
                string hash = GetPropertyHash(property);
                var values = GetCachedValues(cache, hash);
                RemoveNullFromHashValues(values);
                var distinctValues = values.Values.Distinct();
                string currentValue = property.stringValue;
                var filtered = distinctValues.Where(val => val != "" && (currentValue == "" || val.ToLower().Contains(currentValue.ToLower())));
                
                EditorGUI.BeginProperty(position, label, property);

                var rect = EditorGUI.PrefixLabel(position, label);
                string newValue = TextFieldAutocomplete.TextFieldAutoComplete(
                    rect,
                    property.stringValue,
                    filtered.ToArray());
                UpdateCache(property, newValue, values);
                
                EditorGUI.EndProperty();
            }
        }

        private static void UpdateCache(SerializedProperty property, string newValue, Dictionary<ObjectProperty, string> dict)
        {
            property.stringValue = newValue;
            var obj = property.serializedObject.targetObject;
            var target = new ObjectProperty(obj, property.propertyPath);
            dict[target] = newValue;
            
            SaveData();
        }

        private string GetPropertyHash(SerializedProperty property)
        {
            var att = (Attributes.AutocompleteAttribute) attribute;

            if (att.IsCustom)
            {
                return att.CustomHash;
            }
            else
            {
                string path = property.propertyPath;
                path = RemoveArrayIndicesFromPropertyPath(path);
                var targetObject = property.serializedObject.targetObject;
                string typeName = targetObject.GetType().FullName;
                return $"{typeName}.{path}";
            }
        }

        private string RemoveArrayIndicesFromPropertyPath(string path)
        {
            var regx = new Regex(@"\[[0-9]+\]");
            var replaced = regx.Replace(path, "");
            return replaced;
        }
        
        private Dictionary<ObjectProperty, string> GetCachedValues(CacheType cache, string hash)
        {
            if (!cache.ContainsKey(hash))
                cache.Add(hash, new Dictionary<ObjectProperty, string>());
            var values = cache[hash];
            return values;
        }

        private CacheType GetCache()
        {
            var data = Resources.Load<AutocompleteAttributeCacheData>("AutocompleteAttributeCacheData");
            if (!data)
            {
                data = ScriptableObject.CreateInstance<AutocompleteAttributeCacheData>();
                string path = "Assets/Editor/Resources/AutocompleteAttributeCacheData.asset";
                Directory.CreateDirectory(path);
                AssetDatabase.CreateAsset(data, path);
            }
            var cache = data.Cache;
            return cache;
        }

        private static void SaveData()
        {
            var data = Resources.Load<AutocompleteAttributeCacheData>("AutocompleteAttributeCacheData");
            if (!data)
            {
                EditorUtility.SetDirty(data);
            }
        }
        
        private void RemoveNullCachedData(CacheType cache)
        {
            var hashesToRemove = new List<string>();
            foreach (var hash in cache.Keys)
            {
                var dict = cache[hash];
                RemoveNullFromHashValues(dict);

                if (dict.Count == 0)
                {
                    hashesToRemove.Add(hash);
                }
            }

            foreach (var hash in hashesToRemove)
            {
                cache.Remove(hash);
            }
        }

        private static void RemoveNullFromHashValues(Dictionary<ObjectProperty, string> dict)
        {
            var objectPropertiesToRemove = new List<ObjectProperty>();
            foreach (var objectProperty in dict.Keys)
            {
                bool isNull = objectProperty.IsNull();
                if (isNull)
                {
                    objectPropertiesToRemove.Add(objectProperty);
                }
            }

            foreach (var objectProperty in objectPropertiesToRemove)
            {
                dict.Remove(objectProperty);
            }
        }
    }
}