using System;
using UnityEngine;

namespace Utils.Attributes.Animations
{
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = false, Inherited = true)]
    public class AnimatorTransitionAttribute : PropertyAttribute
    {
        public string AnimatorName { get; private set; }

        public AnimatorTransitionAttribute(string animatorName)
        {
            AnimatorName = animatorName;
        }
    }
}