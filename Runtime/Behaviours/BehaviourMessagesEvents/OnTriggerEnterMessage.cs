using UnityEngine;
using UnityEngine.Events;
using Utils.Extensions;

namespace Utils.Behaviours.BehaviourMessagesEvents
{
    public class OnTriggerEnterMessage : MonoBehaviour
    {
        [SerializeField] private LayerMask _mask;
        
        [SerializeField] private UnityEvent<Collider> _onTriggerEnter = new UnityEvent<Collider>();
        
        public UnityEvent<Collider> OnTriggerEnterEvent => _onTriggerEnter;
        
        private void OnTriggerEnter(Collider other)
        {
            if (_mask.IsLayerInMask(other.gameObject.layer))
            {
                _onTriggerEnter.Invoke(other);
            }
        }
    }
}