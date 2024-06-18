using UnityEngine;
using Utils.Serializables;

namespace Samples.Common
{
    public class TypeReferenceTests : MonoBehaviour
    {
        [SerializeField] private TypeReference<Base> type;
        [SerializeField] private TypeReference<IBase> interfaceType;
        [SerializeField] private TypeReference<ISuperBase> superInterfaceType;

        [ContextMenu("Print type")]
        private void PrintType()
        {
            print($"type: {type.Type.Name}");
            print($"interfaceType: {interfaceType.Type.Name}");
            print($"superInterfaceType: {superInterfaceType.Type.Name}");
        }
    }
    
    
    public interface ISuperBase {}
    public interface IBase : ISuperBase {}
    public abstract class Base : IBase {}
    public class ChildA : Base {}
    public class ChildB : Base {}
    public class ChildC : Base {}
}