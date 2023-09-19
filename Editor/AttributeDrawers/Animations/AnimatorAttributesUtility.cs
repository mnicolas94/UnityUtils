using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.Animations;
using UnityEngine;
using Utils.Editor.EditorGUIUtils;

namespace Utils.Editor.AttributeDrawers.Animations
{
    public static class AnimatorAttributesUtility
    {
        public const string InvalidAnimatorControllerWarningMessage = "Target animator controller is null";
        public const string InvalidTypeWarningMessage = "{0} must be an int or a string";
        
        public static float GetPropertyHeight(SerializedProperty property, GUIContent label, string animatorName)
        {
            bool validAnimatorController = GetAnimatorController(property, animatorName) != null;
            bool validPropertyType = property.propertyType is SerializedPropertyType.Integer or SerializedPropertyType.String;

            var normalHeight = EditorGUI.GetPropertyHeight(property, includeChildren: true);
            var warningHeight = normalHeight + GUIUtils.GetHelpBoxHeight();
            
            return validAnimatorController && validPropertyType
                ? normalHeight
                : warningHeight;
        }
        
        public static void OnGUI<T>(Rect rect, SerializedProperty property, GUIContent label, AnimatorController controller,
            List<T> elements, Func<T, int> getIntFunction, Func<T, string> getStringFunction)
        {
            EditorGUI.BeginProperty(rect, label, property);

            if (controller == null)
            {
                GUIUtils.DrawDefaultPropertyAndHelpBox(rect, property, InvalidAnimatorControllerWarningMessage);
                return;
            }

            switch (property.propertyType)
            {
                case SerializedPropertyType.Integer:
                    DrawPropertyForInt(rect, property, label, elements, getIntFunction, getStringFunction);
                    break;
                case SerializedPropertyType.String:
                    DrawPropertyForString(rect, property, label, elements, getStringFunction);
                    break;
                default:
                    GUIUtils.DrawDefaultPropertyAndHelpBox(rect, property, string.Format(InvalidTypeWarningMessage, property.name));
                    break;
            }

            EditorGUI.EndProperty();
        }
        
        
        private static void DrawPropertyForInt<T>(Rect rect, SerializedProperty property, GUIContent label,
            List<T> animatorElements, Func<T, int> getIntFunction, Func<T, string> getNameFunction)
        {
            int paramNameHash = property.intValue;
            int index = 0;

            for (int i = 0; i < animatorElements.Count; i++)
            {
                if (paramNameHash == getIntFunction(animatorElements[i]))
                {
                    index = i + 1; // +1 because the first option is reserved for (None)
                    break;
                }
            }

            string[] displayOptions = GetDisplayOptions(animatorElements, getNameFunction);

            int newIndex = EditorGUI.Popup(rect, label.text, index, displayOptions);
            int newValue = newIndex == 0 ? 0 : getIntFunction(animatorElements[newIndex - 1]);

            if (property.intValue != newValue)
            {
                property.intValue = newValue;
            }
        }

        private static void DrawPropertyForString<T>(Rect rect, SerializedProperty property, GUIContent label,
            List<T> animatorElements, Func<T, string> getStringFunction)
        {
            string paramName = property.stringValue;
            int index = 0;

            for (int i = 0; i < animatorElements.Count; i++)
            {
                if (paramName.Equals(getStringFunction(animatorElements[i]), System.StringComparison.Ordinal))
                {
                    index = i + 1; // +1 because the first option is reserved for (None)
                    break;
                }
            }

            string[] displayOptions = GetDisplayOptions(animatorElements, getStringFunction);

            int newIndex = EditorGUI.Popup(rect, label.text, index, displayOptions);
            string newValue = newIndex == 0 ? null : getStringFunction(animatorElements[newIndex - 1]);

            if (!property.stringValue.Equals(newValue, System.StringComparison.Ordinal))
            {
                property.stringValue = newValue;
            }
        }

        private static string[] GetDisplayOptions<T>(List<T> animatorElements, Func<T, string> getNameFunction)
        {
            string[] displayOptions = new string[animatorElements.Count + 1];
            displayOptions[0] = "(None)";

            for (int i = 0; i < animatorElements.Count; i++)
            {
                displayOptions[i + 1] = getNameFunction(animatorElements[i]);
            }

            return displayOptions;
        }
        
        public static AnimatorController GetAnimatorController(SerializedProperty property, string animatorName)
        {
            var animator = PropertiesUtils.GetFieldByName<Animator>(property, animatorName);
            if (animator != null)
            {
                return GetControllerFromAnimator(animator);
            }

            return null;
        }

        public static AnimatorController GetControllerFromAnimator(Animator animator)
        {
            var runtimeController = animator.runtimeAnimatorController;
            AnimatorController animatorController;
            
            if (runtimeController is AnimatorOverrideController overrideController)
            {
                animatorController = overrideController.runtimeAnimatorController as AnimatorController;
            }
            else
            {
                animatorController = runtimeController as AnimatorController;
            }
            
            return animatorController;
        }
    }
}