using System.Collections.Generic;
using UnityEditor;
using UnityEditor.Animations;
using UnityEngine;
using Utils.Attributes.Animations;

namespace Utils.Editor.AttributeDrawers.Animations
{
    /// <summary>
    /// Taken from https://github.com/dbrizov/NaughtyAttributes/blob/master/Assets/NaughtyAttributes/Scripts/Editor/PropertyDrawers/AnimatorParamPropertyDrawer.cs
    /// Then modified by me.
    /// </summary>
    [CustomPropertyDrawer(typeof(AnimatorParamAttribute))]
    public class AnimatorParamPropertyDrawer : PropertyDrawer
    {
        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            var animatorParamAttribute = (AnimatorParamAttribute) attribute;
            return AnimatorAttributesUtility.GetPropertyHeight(property, label, animatorParamAttribute.AnimatorName);
        }

        public override void OnGUI(Rect rect, SerializedProperty property, GUIContent label)
        {
            var animatorParamAttribute = (AnimatorParamAttribute) attribute;

            AnimatorController animatorController = AnimatorAttributesUtility.GetAnimatorController(property, animatorParamAttribute.AnimatorName);

            List<AnimatorControllerParameter> animatorParameters = new List<AnimatorControllerParameter>();
            if (animatorController)
            {
                int parametersCount = animatorController.parameters.Length;
                for (int i = 0; i < parametersCount; i++)
                {
                    AnimatorControllerParameter parameter = animatorController.parameters[i];
                    if (animatorParamAttribute.AnimatorParamType == null || parameter.type == animatorParamAttribute.AnimatorParamType)
                    {
                        animatorParameters.Add(parameter);
                    }
                }
            }
            
            AnimatorAttributesUtility.OnGUI(rect, property, label, animatorController, animatorParameters,
                GetInt, GetString);
        }

        private int GetInt(AnimatorControllerParameter parameter)
        {
            return parameter.nameHash;
        }

        private string GetString(AnimatorControllerParameter parameter)
        {
            return parameter.name;
        }
    }
}