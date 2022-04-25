using System;
using UnityEngine;

namespace Utils.Attributes
{
    [AttributeUsage(AttributeTargets.Field)]
    public class AutocompleteAttribute : PropertyAttribute
    {
        private string _customHash;
        private bool _isCustom;

        public string CustomHash => _customHash;

        public bool IsCustom => _isCustom;

        public AutocompleteAttribute()
        {
            _isCustom = false;
        }

        public AutocompleteAttribute(string customGroup)
        {
            _customHash = customGroup;
            _isCustom = true;
        }
    }
}