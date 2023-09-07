using System.Collections.Generic;
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
    [CustomPropertyDrawer(typeof(AnimatorTransitionAttribute))]
    public class AnimatorTransitionPropertyDrawer : PropertyDrawer
    {
        private const string InvalidAnimatorControllerWarningMessage = "Target animator controller is null";
        private const string InvalidTypeWarningMessage = "{0} must be an int or a string";

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            var animatorTransitionAttribute = (AnimatorTransitionAttribute) attribute;
            bool validAnimatorController = GetAnimatorController(property, animatorTransitionAttribute.AnimatorName) != null;
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

            var animatorTransitionAttribute = (AnimatorTransitionAttribute) attribute;

            var animatorController = GetAnimatorController(property, animatorTransitionAttribute.AnimatorName);
            if (animatorController == null)
            {
                GUIUtils.DrawDefaultPropertyAndHelpBox(rect, property, InvalidAnimatorControllerWarningMessage);
                return;
            }

            var animatorTransitions = animatorController.GetStateTransitionsWithSource();

            switch (property.propertyType)
            {
                case SerializedPropertyType.Integer:
                    DrawPropertyForInt(rect, property, label, animatorTransitions);
                    break;
                case SerializedPropertyType.String:
                    DrawPropertyForString(rect, property, label, animatorTransitions);
                    break;
                default:
                    GUIUtils.DrawDefaultPropertyAndHelpBox(rect, property, string.Format(InvalidTypeWarningMessage, property.name));
                    break;
            }

            EditorGUI.EndProperty();
        }

        private static void DrawPropertyForInt(Rect rect, SerializedProperty property, GUIContent label, List<(AnimatorStateTransition, AnimatorState)> animatorTransitions)
        {
            int transitionIndex = property.intValue;
            int index = 0;

            for (int i = 0; i < animatorTransitions.Count; i++)
            {
                var (transition, source) = animatorTransitions[i];
                var displayName = transition.GetDisplayName(source);
                if (transitionIndex == Animator.StringToHash(displayName))
                {
                    index = i + 1; // +1 because the first option is reserved for (None)
                    break;
                }
            }

            string[] displayOptions = GetDisplayOptions(animatorTransitions);

            int newIndex = EditorGUI.Popup(rect, label.text, index, displayOptions);
            int newValue = 0;
            if (newIndex > 0)
            {
                var (newTransition, newSource) = animatorTransitions[newIndex - 1];
                var newName = newTransition.GetDisplayName(newSource);
                newValue = Animator.StringToHash(newName);
            }

            if (property.intValue != newValue)
            {
                property.intValue = newValue;
            }
        }

        private static void DrawPropertyForString(Rect rect, SerializedProperty property, GUIContent label, List<(AnimatorStateTransition, AnimatorState)> animatorTransitions)
        {
            string transitionName = property.stringValue;
            int index = 0;

            for (int i = 0; i < animatorTransitions.Count; i++)
            {
                var (transition, source) = animatorTransitions[i];
                var displayName = transition.GetDisplayName(source);
                if (transitionName.Equals(displayName, System.StringComparison.Ordinal))
                {
                    index = i + 1; // +1 because the first option is reserved for (None)
                    break;
                }
            }

            string[] displayOptions = GetDisplayOptions(animatorTransitions);

            int newIndex = EditorGUI.Popup(rect, label.text, index, displayOptions);
            string newValue = null;
            if (newIndex > 0)
            {
                var (newTransition, newSource) = animatorTransitions[newIndex - 1];
                newValue = newTransition.GetDisplayName(newSource);
            }

            if (!property.stringValue.Equals(newValue, System.StringComparison.Ordinal))
            {
                property.stringValue = newValue;
            }
        }

        private static string[] GetDisplayOptions(List<(AnimatorStateTransition, AnimatorState)> animatorTransitions)
        {
            string[] displayOptions = new string[animatorTransitions.Count + 1];
            displayOptions[0] = "(None)";

            for (int i = 0; i < animatorTransitions.Count; i++)
            {
                var (transition, source) = animatorTransitions[i];
                var displayName = transition.GetDisplayName(source);
                displayOptions[i + 1] = displayName;
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