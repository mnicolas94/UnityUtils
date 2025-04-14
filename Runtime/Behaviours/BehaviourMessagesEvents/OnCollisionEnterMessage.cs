using UnityEngine;
using UnityEngine.Events;
using Utils.Extensions;

namespace Utils.Behaviours.BehaviourMessagesEvents
{
    public class OnCollisionEnterMessage : MonoBehaviour
    {
        [SerializeField] private LayerMask _mask = -1;
        public LayerMask Mask
        {
            get => _mask;
            set => _mask = value;
        }

        [SerializeField] private UnityEvent<Collision> _onCollisionEnter = new UnityEvent<Collision>();
        public UnityEvent<Collision> OnCollisionEnterEvent => _onCollisionEnter;
        
        private void OnCollisionEnter(Collision collision)
        {
            if (_mask.IsLayerInMask(collision.gameObject.layer))
            {
                _onCollisionEnter.Invoke(collision);
            }
        }
    }
}