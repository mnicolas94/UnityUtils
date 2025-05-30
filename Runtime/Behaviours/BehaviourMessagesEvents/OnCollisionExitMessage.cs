using UnityEngine;
using UnityEngine.Events;
using Utils.Extensions;

namespace Utils.Behaviours.BehaviourMessagesEvents
{
    public class OnCollisionExitMessage : MonoBehaviour
    {
        [SerializeField] private LayerMask _mask = -1;
        public LayerMask Mask
        {
            get => _mask;
            set => _mask = value;
        }
        
        [SerializeField] private UnityEvent<Collision> _onCollisionExit = new UnityEvent<Collision>();
        
        public UnityEvent<Collision> OnCollisionExitEvent => _onCollisionExit;
        
        private void OnCollisionExit(Collision other)
        {
            if (_mask.IsLayerInMask(other.gameObject.layer))
            {
                _onCollisionExit.Invoke(other);
            }
        }
    }
}