using System;
using UnityEngine;

namespace Samples.FixSerializedReferences
{
    [Serializable]
    public class ReferenceBase
    {
        [SerializeField] private int _base;
    }

    [Serializable]
    public class ReferenceA : ReferenceBase
    {
        [SerializeField] private string _a;
    }
    
    [Serializable]
    public class ReferenceB : ReferenceBase
    {
        [SerializeField] private float _b;
    }
}