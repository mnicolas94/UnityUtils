using System;
using UnityEngine;

namespace Utils.Attributes.Animations
{
    /// <summary>
    /// Taken from https://github.com/dbrizov/NaughtyAttributes/blob/master/Assets/NaughtyAttributes/Scripts/Core/DrawerAttributes/AnimatorParamAttribute.cs
    /// </summary>
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = false, Inherited = true)]
    public class AnimatorParamAttribute : PropertyAttribute
    {
        public string AnimatorName { get; private set; }
        public AnimatorControllerParameterType? AnimatorParamType { get; private set; }

        public AnimatorParamAttribute(string animatorName)
        {
            AnimatorName = animatorName;
            AnimatorParamType = null;
        }

        public AnimatorParamAttribute(string animatorName, AnimatorControllerParameterType animatorParamType)
        {
            AnimatorName = animatorName;
            AnimatorParamType = animatorParamType;
        }
    }
}