using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using Utils.Editor;

namespace Samples
{
    public class IterateSerializedPropertySample : MonoBehaviour
    {
        [SerializeField] private int _int;
        [SerializeField] private string _string;
        [SerializeField] private List<int> _list;

        [SerializeField] private bool _recursive;
        [SerializeField] private bool _returnScript;
        
        [ContextMenu("Debug")]
        public void DebugSps()
        {
            var so = new SerializedObject(this);
            foreach (var sp in PropertiesUtils.GetSerializedProperties(so, _returnScript, _recursive))
            {
                Debug.Log(sp.name);
            }
        }
    }
}