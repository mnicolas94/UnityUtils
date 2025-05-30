﻿using UnityEngine;
using UnityEngine.Events;
using Utils.Extensions;

namespace Utils.Behaviours.BehaviourMessagesEvents
{
    public class OnTriggerEnterMessage : MonoBehaviour
    {
        [SerializeField] private LayerMask _mask = -1;
        public LayerMask Mask
        {
            get => _mask;
            set => _mask = value;
        }
        
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