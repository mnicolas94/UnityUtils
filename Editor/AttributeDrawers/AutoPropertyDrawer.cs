using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;
using Utils.Attributes;
using Object = UnityEngine.Object;

namespace Utils.Editor.AttributeDrawers
{
    /// <summary>
    /// Some code was taken from: https://github.com/Deadcows/MyBox
    /// </summary>
    [CustomPropertyDrawer(typeof(AutoPropertyAttribute))]
    public class AutoPropertyDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property,
            GUIContent label)
        {
            bool isUnityObject = true;
            bool isEmpty = property.objectReferenceValue == null;
            if (isUnityObject && isEmpty)
            {
                var att = (AutoPropertyAttribute) attribute;
                var mode = att.Mode;
                Func<Object, bool> predicateMethod = GetPredicateMethod(property, att);
                var objects = GetObjectsFromAutoPropertyMode(mode, property, predicateMethod, fieldInfo.FieldType);
                var obj = objects.FirstOrDefault();
                if (obj != null)
                {
                    property.objectReferenceValue = obj;
                    return;
                }
            }
                    
            EditorGUI.PropertyField(position, property, label);
        }

        private static Func<Object, bool> GetPredicateMethod(SerializedProperty property, AutoPropertyAttribute att)
        {
            Func<Object, bool> function;
            if (att.PredicateMethodName != null)
            {
                Type targetType = att.PredicateMethodTarget;
                if (targetType != null)
                {
                    var bindingFlags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance;
                    var methodInfo = targetType.GetMethod(att.PredicateMethodName, bindingFlags);
                    function = (Func<Object, bool>) methodInfo.CreateDelegate(typeof(Func<Object, bool>), null);
//                    function = (Func<Object, bool>) Delegate.CreateDelegate(typeof(Func<Object, bool>),
//                        att.PredicateMethodTarget,
//                        att.PredicateMethodName);
                }
                else
                {
                    var target = property.serializedObject.targetObject;
                    function = (Func<Object, bool>) Delegate.CreateDelegate(typeof(Func<Object, bool>),
                        target, att.PredicateMethodName);
                }
            }
            else
            {
                function = _ => true;
            }

            return function;
        }

        private static IEnumerable<Object> GetObjectsFromAutoPropertyMode(
            AutoPropertyMode mode,
            SerializedProperty property,
            Func<Object, bool> predicate,
            Type type
        )
        {
            IEnumerable<Object> objects;
            switch (mode)
            {
                case AutoPropertyMode.Children:
                    objects = ((Component) property.serializedObject.targetObject)
                        ?.GetComponentsInChildren(type, true);
                    break;
                case AutoPropertyMode.Parent:
                    objects = ((Component) property.serializedObject.targetObject)
                        ?.GetComponentsInParent(type, true);
                    break;
                case AutoPropertyMode.Scene:
                    objects = GetAllComponentsInSceneOf(property.serializedObject.targetObject, type);
                    break;
                case AutoPropertyMode.Asset:
                    objects = AssetDatabase.FindAssets($"t:{type.Name}")
                        .Select(AssetDatabase.GUIDToAssetPath)
                        .Select(AssetDatabase.LoadAssetAtPath<Object>);
                    break;
                default:
                case AutoPropertyMode.Any:
                    var sceneObjects = GetAllComponentsInSceneOf(property.serializedObject.targetObject, type);
                    var assets = AssetDatabase.FindAssets($"t:{type.Name}")
                        .Select(AssetDatabase.GUIDToAssetPath)
                        .Select(AssetDatabase.LoadAssetAtPath<Object>);
                    objects = sceneObjects.Concat(assets);
                    break;
            }
            
            return objects.Where(predicate);
        }
            
        /// <summary>
        /// Get all Components in the same scene as a specified GameObject,
        /// including inactive components.
        /// 
        /// source: https://github.com/Deadcows/MyBox/blob/master/Extensions/EditorExtensions/MyEditor.cs
        /// </summary>
        public static IEnumerable<Component> GetAllComponentsInSceneOf(Object obj,
            Type type)
        {
            GameObject contextGO;
            if (obj is Component comp) contextGO = comp.gameObject;
            else if (obj is GameObject go) contextGO = go;
            else return Array.Empty<Component>();
            if (contextGO.scene.isLoaded) return contextGO.scene.GetRootGameObjects()
                .SelectMany(rgo => rgo.GetComponentsInChildren(type, true));
            return Array.Empty<Component>();
        }
    }
}