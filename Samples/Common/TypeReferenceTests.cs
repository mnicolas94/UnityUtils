using UnityEngine;
using Utils.Serializables;

namespace Samples.Common
{
    public class TypeReferenceTests : MonoBehaviour
    {
        [SerializeField] private TypeReference<Base> type;
        [SerializeField] private En en;

        [ContextMenu("Print type")]
        private void PrintType()
        {
            print($"type: {type.Type.Name}");
        }
    }
    
    public enum En { A, B, C}
    
    public class Base {}
    public class ChildA : Base {}
    public class ChildB : Base {}
    public class ChildC : Base {}
}