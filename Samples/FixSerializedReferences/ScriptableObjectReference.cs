using UnityEngine;

namespace Samples.FixSerializedReferences
{
    public class ScriptableObjectReference : ScriptableObject
    {
        [SerializeReference] private ReferenceBase _reference;
    }
}