using System.Collections.Generic;
using UnityEngine;
using Utils.Attributes;

namespace Samples.Common
{
    [CreateAssetMenu(fileName = "DropdownSample", menuName = "Facticus/Utils/Samples/DropdownAttribute", order = 0)]
    public class ScriptableDropdownAttributeSample : ScriptableObject
    {
        [SerializeField, Dropdown(nameof(Values))]
        private string _value;
        private List<string> Values()
        {
            return new List<string>()
            {
                "A",
                "B",
                "C",
                "D",
            };
        }
    }
}