﻿using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEditor.Animations;
using UnityEngine;
using Utils.Attributes.Animations;
using Utils.Editor.EditorGUIUtils;
using Utils.Extensions;

namespace Utils.Editor.AttributeDrawers.Animations
{
    [CustomPropertyDrawer(typeof(AnimatorStateAttribute))]
    public class AnimatorStatePropertyDrawer : PropertyDrawer
    {
        private const string InvalidAnimatorControllerWarningMessage = "Target animator controller is null";
        private const string InvalidTypeWarningMessage = "{0} must be an int or a string";

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            var animatorStateAttribute = (AnimatorStateAttribute) attribute;
            bool validAnimatorController = GetAnimatorController(property, animatorStateAttribute.AnimatorName) != null;
            bool validPropertyType = property.propertyType is SerializedPropertyType.Integer or SerializedPropertyType.String;

            var normalHeight = EditorGUI.GetPropertyHeight(property, includeChildren: true);
            var warningHeight = normalHeight + GUIUtils.GetHelpBoxHeight();
            
            return validAnimatorController && validPropertyType
                ? normalHeight
                : warningHeight;
        }

        public override void OnGUI(Rect rect, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(rect, label, property);

            var animatorStateAttribute = (AnimatorStateAttribute) attribute;

            var animatorController = GetAnimatorController(property, animatorStateAttribute.AnimatorName);
            if (animatorController == null)
            {
                GUIUtils.DrawDefaultPropertyAndHelpBox(rect, property, InvalidAnimatorControllerWarningMessage);
                return;
            }

            var animatorStates = animatorController.GetStates();

            switch (property.propertyType)
            {
                case SerializedPropertyType.Integer:
                    DrawPropertyForInt(rect, property, label, animatorStates);
                    break;
                case SerializedPropertyType.String:
                    DrawPropertyForString(rect, property, label, animatorStates);
                    break;
                default:
                    GUIUtils.DrawDefaultPropertyAndHelpBox(rect, property, string.Format(InvalidTypeWarningMessage, property.name));
                    break;
            }

            EditorGUI.EndProperty();
        }

        private static void DrawPropertyForInt(Rect rect, SerializedProperty property, GUIContent label, List<AnimatorState> animatorStates)
        {
            int stateIndex = property.intValue;
            int index = 0;

            for (int i = 0; i < animatorStates.Count; i++)
            {
                if (stateIndex == animatorStates[i].nameHash)
                {
                    index = i + 1; // +1 because the first option is reserved for (None)
                    break;
                }
            }

            string[] displayOptions = GetDisplayOptions(animatorStates);

            int newIndex = EditorGUI.Popup(rect, label.text, index, displayOptions);
            int newValue = newIndex == 0 ? 0 : animatorStates[newIndex - 1].nameHash;

            if (property.intValue != newValue)
            {
                property.intValue = newValue;
            }
        }

        private static void DrawPropertyForString(Rect rect, SerializedProperty property, GUIContent label, List<AnimatorState> animatorStates)
        {
            string stateName = property.stringValue;
            int index = 0;

            for (int i = 0; i < animatorStates.Count; i++)
            {
                if (stateName.Equals(animatorStates[i].name, System.StringComparison.Ordinal))
                {
                    index = i + 1; // +1 because the first option is reserved for (None)
                    break;
                }
            }

            string[] displayOptions = GetDisplayOptions(animatorStates);

            int newIndex = EditorGUI.Popup(rect, label.text, index, displayOptions);
            string newValue = newIndex == 0 ? null : animatorStates[newIndex - 1].name;

            if (!property.stringValue.Equals(newValue, System.StringComparison.Ordinal))
            {
                property.stringValue = newValue;
            }
        }

        private static string[] GetDisplayOptions(List<AnimatorState> animatorStates)
        {
            string[] displayOptions = new string[animatorStates.Count + 1];
            displayOptions[0] = "(None)";

            for (int i = 0; i < animatorStates.Count; i++)
            {
                displayOptions[i + 1] = animatorStates[i].name;
            }

            return displayOptions;
        }

        private static AnimatorController GetAnimatorController(SerializedProperty property, string animatorName)
        {
            object target = PropertiesUtils.GetTargetObjectWithProperty(property);

            FieldInfo animatorFieldInfo = ReflectionUtility.GetField(target, animatorName);
            if (animatorFieldInfo != null &&
                animatorFieldInfo.FieldType == typeof(Animator))
            {
                Animator animator = animatorFieldInfo.GetValue(target) as Animator;
                if (animator != null)
                {
                    AnimatorController animatorController = animator.runtimeAnimatorController as AnimatorController;
                    return animatorController;
                }
            }

            PropertyInfo animatorPropertyInfo = ReflectionUtility.GetProperty(target, animatorName);
            if (animatorPropertyInfo != null &&
                animatorPropertyInfo.PropertyType == typeof(Animator))
            {
                Animator animator = animatorPropertyInfo.GetValue(target) as Animator;
                if (animator != null)
                {
                    AnimatorController animatorController = animator.runtimeAnimatorController as AnimatorController;
                    return animatorController;
                }
            }

            MethodInfo animatorGetterMethodInfo = ReflectionUtility.GetMethod(target, animatorName);
            if (animatorGetterMethodInfo != null &&
                animatorGetterMethodInfo.ReturnType == typeof(Animator) &&
                animatorGetterMethodInfo.GetParameters().Length == 0)
            {
                Animator animator = animatorGetterMethodInfo.Invoke(target, null) as Animator;
                if (animator != null)
                {
                    AnimatorController animatorController = animator.runtimeAnimatorController as AnimatorController;
                    return animatorController;
                }
            }

            return null;
        }

        
    }
}