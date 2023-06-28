using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;
using Utils.Attributes;

namespace Samples.Common
{
    public class DropdownAttributeSample : MonoBehaviour
    {
        [SerializeField, Dropdown(nameof(Values))]
        private string _value;

        [SerializeReference] private IManaged _managed = new Managed();
        
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

        [ContextMenu("Debug value")]
        private void DebugValue()
        {
            Debug.Log(_value);
        }
    }

    public interface IManaged
    {
        
    }
    
    [Serializable]
    public class Managed : IManaged
    {
        [SerializeField, Dropdown(nameof(Values))]
        private string _value;
        
        [SerializeField, Dropdown(nameof(Values))]
        private string _value2;

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
    
#if UNITY_EDITOR
    [CustomEditor(typeof(DropdownAttributeSample))]
    public class DropdownAttributeSampleEditor : Editor
    {
        public override VisualElement CreateInspectorGUI()
        {
            var root = new VisualElement();
            
            InspectorElement.FillDefaultInspector(root, serializedObject, this);

            return root;
        }
    }
#endif
}
