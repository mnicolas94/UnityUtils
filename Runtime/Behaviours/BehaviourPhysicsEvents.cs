using System;
using UnityEngine;
using UnityEngine.Events;
using Utils.Extensions;

namespace Utils.Behaviours
{
    [Obsolete("Use OnXXXMessage components for each MonoBehaviour message. It will be better organized in editor.")]
    public class BehaviourPhysicsEvents : MonoBehaviour
    {
        [SerializeField] private LayerMask _mask;
        [SerializeField] private UnityEvent<Collider> _onTriggerEnter = new UnityEvent<Collider>();
        [SerializeField] private UnityEvent<Collider> _onTriggerExit = new UnityEvent<Collider>();
        [SerializeField] private UnityEvent<Collider2D> _onTriggerEnter2D = new UnityEvent<Collider2D>();
        [SerializeField] private UnityEvent<Collider2D> _onTriggerExit2D = new UnityEvent<Collider2D>();
        [SerializeField] private UnityEvent<Collision> _onCollisionEnter = new UnityEvent<Collision>();
        [SerializeField] private UnityEvent<Collision> _onCollisionExit = new UnityEvent<Collision>();
        [SerializeField] private UnityEvent<Collision2D> _onCollisionEnter2D = new UnityEvent<Collision2D>();
        [SerializeField] private UnityEvent<Collision2D> _onCollisionExit2D = new UnityEvent<Collision2D>();

        public UnityEvent<Collider> OnTriggerEnterEvent => _onTriggerEnter;

        public UnityEvent<Collider> OnTriggerExitEvent => _onTriggerExit;

        public UnityEvent<Collider2D> OnTriggerEnter2DEvent => _onTriggerEnter2D;

        public UnityEvent<Collider2D> OnTriggerExit2DEvent => _onTriggerExit2D;
        
        public UnityEvent<Collision> OnCollisionEnterEvent => _onCollisionEnter;

        public UnityEvent<Collision> OnCollisionExitEvent => _onCollisionExit;

        public UnityEvent<Collision2D> OnCollisionEnter2DEvent => _onCollisionEnter2D;

        public UnityEvent<Collision2D> OnCollisionExit2DEvent => _onCollisionExit2D;

        private void OnTriggerEnter(Collider other)
        {
            if (_mask.IsLayerInMask(other.gameObject.layer))
            {
                _onTriggerEnter.Invoke(other);
            }
        }

        private void OnTriggerExit(Collider other)
        {
            if (_mask.IsLayerInMask(other.gameObject.layer))
            {
                _onTriggerExit.Invoke(other);
            }
        }

        private void OnTriggerEnter2D(Collider2D col)
        {
            if (_mask.IsLayerInMask(col.gameObject.layer))
            {
                _onTriggerEnter2D.Invoke(col);
            }
        }

        private void OnTriggerExit2D(Collider2D other)
        {
            if (_mask.IsLayerInMask(other.gameObject.layer))
            {
                _onTriggerExit2D.Invoke(other);
            }
        }
        
        private void OnCollisionEnter(Collision collision)
        {
            if (_mask.IsLayerInMask(collision.gameObject.layer))
            {
                _onCollisionEnter.Invoke(collision);
            }
        }

        private void OnCollisionExit(Collision other)
        {
            if (_mask.IsLayerInMask(other.gameObject.layer))
            {
                _onCollisionExit.Invoke(other);
            }
        }

        private void OnCollisionEnter2D(Collision2D col)
        {
            if (_mask.IsLayerInMask(col.gameObject.layer))
            {
                _onCollisionEnter2D.Invoke(col);
            }
        }

        private void OnCollisionExit2D(Collision2D other)
        {
            if (_mask.IsLayerInMask(other.gameObject.layer))
            {
                _onCollisionExit2D.Invoke(other);
            }
        }
    }
}