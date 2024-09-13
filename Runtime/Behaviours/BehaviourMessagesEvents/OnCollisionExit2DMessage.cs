using UnityEngine;
using UnityEngine.Events;
using Utils.Extensions;

namespace Utils.Behaviours.BehaviourMessagesEvents
{
    public class OnCollisionExit2DMessage : MonoBehaviour
    {
        [SerializeField] private LayerMask _mask;
        public LayerMask Mask
        {
            get => _mask;
            set => _mask = value;
        }
        
        [SerializeField] private UnityEvent<Collision2D> _onCollisionExit2D = new UnityEvent<Collision2D>();
        
        public UnityEvent<Collision2D> OnCollisionExit2DEvent => _onCollisionExit2D;
        
        private void OnCollisionExit2D(Collision2D other)
        {
            if (_mask.IsLayerInMask(other.gameObject.layer))
            {
                _onCollisionExit2D.Invoke(other);
            }
        }
    }
}