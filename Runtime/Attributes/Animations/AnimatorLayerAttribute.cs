using System;
using UnityEngine;

namespace Utils.Attributes.Animations
{
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = false, Inherited = true)]
    public class AnimatorLayerAttribute : PropertyAttribute
    {
        public string AnimatorName { get; private set; }

        public AnimatorLayerAttribute(string animatorName)
        {
            AnimatorName = animatorName;
        }
    }
}