using System;
using UnityEngine;

namespace Utils.Attributes.Animations
{
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = false, Inherited = true)]
    public class AnimatorStateAttribute : PropertyAttribute
    {
        public string AnimatorName { get; private set; }

        public AnimatorStateAttribute(string animatorName)
        {
            AnimatorName = animatorName;
        }
    }
}