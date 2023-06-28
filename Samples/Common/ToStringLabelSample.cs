using System;
using System.Collections.Generic;
using UnityEngine;
using Utils.Attributes;

namespace Samples.Common
{
    public class ToStringLabelSample : MonoBehaviour
    {
        [SerializeField, ToStringLabel] private CustomClass _a;
        [SerializeField, ToStringLabel] private CustomClassDate _d;
        
        [SerializeField, ToStringLabel] private List<CustomClass> _la;
        [SerializeField, ToStringLabel] private List<CustomClassDate> _ld;
    }

    [Serializable]
    public class CustomClass
    {
        [SerializeField] private string _s;
        [SerializeField] private float _f;
        
        public override string ToString()
        {
            return $"{_s}: {_f}";
        }
    }
    
    [Serializable]
    public class CustomClassDate
    {
        public override string ToString()
        {
            return DateTime.Now.ToLongTimeString();
        }
    }
}