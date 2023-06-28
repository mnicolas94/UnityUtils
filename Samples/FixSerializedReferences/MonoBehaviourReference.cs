using UnityEngine;

namespace Samples.FixSerializedReferences
{
    public class MonoBehaviourReference : MonoBehaviour
    {
        [SerializeReference] private ReferenceBase _reference;

        [ContextMenu("Set reference")]
        private void SetReference()
        {
            _reference = new ReferenceB();
        }
    }
}