using UnityEngine;
using UnityEngine.Events;
using Utils.Extensions;

namespace Utils.Behaviours.BehaviourMessagesEvents
{
    public class OnTriggerExit2DMessage : MonoBehaviour
    {
        [SerializeField] private LayerMask _mask = -1;
        public LayerMask Mask
        {
            get => _mask;
            set => _mask = value;
        }
        
        [SerializeField] private UnityEvent<Collider2D> _onTriggerExit2D = new UnityEvent<Collider2D>();
        
        public UnityEvent<Collider2D> OnTriggerExit2DEvent => _onTriggerExit2D;
        
        private void OnTriggerExit2D(Collider2D other)
        {
            if (_mask.IsLayerInMask(other.gameObject.layer))
            {
                _onTriggerExit2D.Invoke(other);
            }
        }
    }
}