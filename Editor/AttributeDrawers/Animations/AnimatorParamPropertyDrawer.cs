using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
using UnityEditor.Animations;
using UnityEngine;
using Utils.Attributes.Animations;
using Utils.Editor.EditorGUIUtils;

namespace Utils.Editor.AttributeDrawers.Animations
{
    /// <summary>
    /// Taken from https://github.com/dbrizov/NaughtyAttributes/blob/master/Assets/NaughtyAttributes/Scripts/Editor/PropertyDrawers/AnimatorParamPropertyDrawer.cs
    /// Then modified by me.
    /// </summary>
    [CustomPropertyDrawer(typeof(AnimatorParamAttribute))]
    public class AnimatorParamPropertyDrawer : PropertyDrawer
    {
        private const string InvalidAnimatorControllerWarningMessage = "Target animator controller is null";
        private const string InvalidTypeWarningMessage = "{0} must be an int or a string";

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            var animatorParamAttribute = (AnimatorParamAttribute) attribute;
            bool validAnimatorController = GetAnimatorController(property, animatorParamAttribute.AnimatorName) != null;
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

            var animatorParamAttribute = (AnimatorParamAttribute) attribute;

            AnimatorController animatorController = GetAnimatorController(property, animatorParamAttribute.AnimatorName);
            if (animatorController == null)
            {
                GUIUtils.DrawDefaultPropertyAndHelpBox(rect, property, InvalidAnimatorControllerWarningMessage);
                return;
            }

            int parametersCount = animatorController.parameters.Length;
            List<AnimatorControllerParameter> animatorParameters = new List<AnimatorControllerParameter>(parametersCount);
            for (int i = 0; i < parametersCount; i++)
            {
                AnimatorControllerParameter parameter = animatorController.parameters[i];
                if (animatorParamAttribute.AnimatorParamType == null || parameter.type == animatorParamAttribute.AnimatorParamType)
                {
                    animatorParameters.Add(parameter);
                }
            }

            switch (property.propertyType)
            {
                case SerializedPropertyType.Integer:
                    DrawPropertyForInt(rect, property, label, animatorParameters);
                    break;
                case SerializedPropertyType.String:
                    DrawPropertyForString(rect, property, label, animatorParameters);
                    break;
                default:
                    GUIUtils.DrawDefaultPropertyAndHelpBox(rect, property, string.Format(InvalidTypeWarningMessage, property.name));
                    break;
            }

            EditorGUI.EndProperty();
        }

        private static void DrawPropertyForInt(Rect rect, SerializedProperty property, GUIContent label, List<AnimatorControllerParameter> animatorParameters)
        {
            int paramNameHash = property.intValue;
            int index = 0;

            for (int i = 0; i < animatorParameters.Count; i++)
            {
                if (paramNameHash == animatorParameters[i].nameHash)
                {
                    index = i + 1; // +1 because the first option is reserved for (None)
                    break;
                }
            }

            string[] displayOptions = GetDisplayOptions(animatorParameters);

            int newIndex = EditorGUI.Popup(rect, label.text, index, displayOptions);
            int newValue = newIndex == 0 ? 0 : animatorParameters[newIndex - 1].nameHash;

            if (property.intValue != newValue)
            {
                property.intValue = newValue;
            }
        }

        private static void DrawPropertyForString(Rect rect, SerializedProperty property, GUIContent label, List<AnimatorControllerParameter> animatorParameters)
        {
            string paramName = property.stringValue;
            int index = 0;

            for (int i = 0; i < animatorParameters.Count; i++)
            {
                if (paramName.Equals(animatorParameters[i].name, System.StringComparison.Ordinal))
                {
                    index = i + 1; // +1 because the first option is reserved for (None)
                    break;
                }
            }

            string[] displayOptions = GetDisplayOptions(animatorParameters);

            int newIndex = EditorGUI.Popup(rect, label.text, index, displayOptions);
            string newValue = newIndex == 0 ? null : animatorParameters[newIndex - 1].name;

            if (!property.stringValue.Equals(newValue, System.StringComparison.Ordinal))
            {
                property.stringValue = newValue;
            }
        }

        private static string[] GetDisplayOptions(List<AnimatorControllerParameter> animatorParams)
        {
            string[] displayOptions = new string[animatorParams.Count + 1];
            displayOptions[0] = "(None)";

            for (int i = 0; i < animatorParams.Count; i++)
            {
                displayOptions[i + 1] = animatorParams[i].name;
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