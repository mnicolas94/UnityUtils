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

            var animatorStates = GetStates(animatorController);

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

        private static void DrawPropertyForInt(Rect rect, SerializedProperty property, GUIContent label, List<AnimatorState> animatorLayers)
        {
            int layerIndex = property.intValue;
            int index = 0;

            for (int i = 0; i < animatorLayers.Count; i++)
            {
                if (layerIndex == animatorLayers[i].nameHash)
                {
                    index = i + 1; // +1 because the first option is reserved for (None)
                    break;
                }
            }

            string[] displayOptions = GetDisplayOptions(animatorLayers);

            int newIndex = EditorGUI.Popup(rect, label.text, index, displayOptions);
            int newValue = newIndex == 0 ? 0 : animatorLayers[newIndex - 1].nameHash;

            if (property.intValue != newValue)
            {
                property.intValue = newValue;
            }
        }

        private static void DrawPropertyForString(Rect rect, SerializedProperty property, GUIContent label, List<AnimatorState> animatorLayers)
        {
            string layerName = property.stringValue;
            int index = 0;

            for (int i = 0; i < animatorLayers.Count; i++)
            {
                if (layerName.Equals(animatorLayers[i].name, System.StringComparison.Ordinal))
                {
                    index = i + 1; // +1 because the first option is reserved for (None)
                    break;
                }
            }

            string[] displayOptions = GetDisplayOptions(animatorLayers);

            int newIndex = EditorGUI.Popup(rect, label.text, index, displayOptions);
            string newValue = newIndex == 0 ? null : animatorLayers[newIndex - 1].name;

            if (!property.stringValue.Equals(newValue, System.StringComparison.Ordinal))
            {
                property.stringValue = newValue;
            }
        }

        private static string[] GetDisplayOptions(List<AnimatorState> animatorLayers)
        {
            string[] displayOptions = new string[animatorLayers.Count + 1];
            displayOptions[0] = "(None)";

            for (int i = 0; i < animatorLayers.Count; i++)
            {
                displayOptions[i + 1] = animatorLayers[i].name;
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

        private List<AnimatorState> GetStates(AnimatorController controller)
        {
            int layersCount = controller.layers.Length;
            var animatorStates = new List<AnimatorState>(layersCount);
            
            var stateMachines = controller.layers.Select(layer => layer.stateMachine).ToList();

            while (stateMachines.Count > 0)
            {
                var stateMachine = stateMachines[0];
                stateMachines.RemoveAt(0);
                
                // add children state machines
                var subMachines = stateMachine.stateMachines.Select(sm => sm.stateMachine);
                stateMachines.AddRange(subMachines);
                
                // add states
                var states = stateMachine.states.Select(s => s.state);
                foreach (var state in states)
                {
                    var alreadyAdded = animatorStates.Exists(s => s.nameHash == state.nameHash);
                    if (!alreadyAdded)
                    {
                        animatorStates.Add(state);
                    }
                }
            }

            return animatorStates;
        }
    }
}