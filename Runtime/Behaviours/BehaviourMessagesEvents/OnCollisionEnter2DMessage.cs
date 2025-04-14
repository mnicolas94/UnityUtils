using UnityEngine;
using UnityEngine.Events;
using Utils.Extensions;

namespace Utils.Behaviours.BehaviourMessagesEvents
{
    public class OnCollisionEnter2DMessage : MonoBehaviour
    {
        [SerializeField] private LayerMask _mask = -1;
        public LayerMask Mask
        {
            get => _mask;
            set => _mask = value;
        }
        
        [SerializeField] private UnityEvent<Collision2D> _onCollisionEnter2D = new UnityEvent<Collision2D>();
        
        public UnityEvent<Collision2D> OnCollisionEnter2DEvent => _onCollisionEnter2D;
        
        private void OnCollisionEnter2D(Collision2D col)
        {
            if (_mask.IsLayerInMask(col.gameObject.layer))
            {
                _onCollisionEnter2D.Invoke(col);
            }
        }
    }
}