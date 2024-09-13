using UnityEngine;
using UnityEngine.Events;
using Utils.Extensions;

namespace Utils.Behaviours.BehaviourMessagesEvents
{
    public class OnTriggerExitMessage : MonoBehaviour
    {
        [SerializeField] private LayerMask _mask;
        public LayerMask Mask
        {
            get => _mask;
            set => _mask = value;
        }
        
        [SerializeField] private UnityEvent<Collider> _onTriggerExit = new UnityEvent<Collider>();
        
        public UnityEvent<Collider> OnTriggerExitEvent => _onTriggerExit;
        
        private void OnTriggerExit(Collider other)
        {
            if (_mask.IsLayerInMask(other.gameObject.layer))
            {
                _onTriggerExit.Invoke(other);
            }
        }
    }
}