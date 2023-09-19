using UnityEditor;
using UnityEditor.Animations;
using UnityEngine;
using Utils.Attributes.Animations;

namespace Utils.Editor.AttributeDrawers.Animations
{
    [CustomPropertyDrawer(typeof(AnimatorStateAttribute))]
    public class AnimatorStatePropertyDrawer : PropertyDrawer
    {
        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            var animatorStateAttribute = (AnimatorStateAttribute) attribute;
            return AnimatorAttributesUtility.GetPropertyHeight(property, label, animatorStateAttribute.AnimatorName);
        }

        public override void OnGUI(Rect rect, SerializedProperty property, GUIContent label)
        {
            var animatorStateAttribute = (AnimatorStateAttribute) attribute;

            var animatorController = AnimatorAttributesUtility.GetAnimatorController(property, animatorStateAttribute.AnimatorName);

            var animatorStates = animatorController.GetStates();

            AnimatorAttributesUtility.OnGUI(rect, property, label, animatorController, animatorStates,
                GetInt, GetString);
        }

        private int GetInt(AnimatorState parameter)
        {
            return parameter.nameHash;
        }

        private string GetString(AnimatorState parameter)
        {
            return parameter.name;
        }
    }
}