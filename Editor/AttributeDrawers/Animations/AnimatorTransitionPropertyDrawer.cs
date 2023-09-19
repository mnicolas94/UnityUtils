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
        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            var animatorTransitionAttribute = (AnimatorTransitionAttribute) attribute;
            return AnimatorAttributesUtility.GetPropertyHeight(property, label, animatorTransitionAttribute.AnimatorName);
        }

        public override void OnGUI(Rect rect, SerializedProperty property, GUIContent label)
        {
            var animatorTransitionAttribute = (AnimatorTransitionAttribute) attribute;

            var animatorController = AnimatorAttributesUtility.GetAnimatorController(property, animatorTransitionAttribute.AnimatorName);

            var animatorTransitions = animatorController ? animatorController.GetStateTransitionsWithSource() : new List<(AnimatorStateTransition, AnimatorState)>();

            AnimatorAttributesUtility.OnGUI(rect, property, label, animatorController, animatorTransitions,
                GetInt, GetString);
        }

        private int GetInt((AnimatorStateTransition, AnimatorState) transitionSource)
        {
            var (transition, source) = transitionSource;
            var displayName = transition.GetDisplayName(source);
            return Animator.StringToHash(displayName);
        }

        private string GetString((AnimatorStateTransition, AnimatorState) transitionSource)
        {
            var (transition, source) = transitionSource;
            var displayName = transition.GetDisplayName(source);
            return displayName;
        }
    }
}