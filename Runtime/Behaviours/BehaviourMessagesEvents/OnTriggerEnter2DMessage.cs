using UnityEngine;
using UnityEngine.Events;
using Utils.Extensions;

namespace Utils.Behaviours.BehaviourMessagesEvents
{
    public class OnTriggerEnter2DMessage : MonoBehaviour
    {
        [SerializeField] private LayerMask _mask;
        public LayerMask Mask
        {
            get => _mask;
            set => _mask = value;
        }
        
        [SerializeField] private UnityEvent<Collider2D> _onTriggerEnter2D = new UnityEvent<Collider2D>();
        
        public UnityEvent<Collider2D> OnTriggerEnter2DEvent => _onTriggerEnter2D;
        
        private void OnTriggerEnter2D(Collider2D col)
        {
            if (_mask.IsLayerInMask(col.gameObject.layer))
            {
                _onTriggerEnter2D.Invoke(col);
            }
        }
    }
}