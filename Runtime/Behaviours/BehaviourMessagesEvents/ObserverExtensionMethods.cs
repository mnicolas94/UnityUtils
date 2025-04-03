using System;
using UnityEngine;
using UnityEngine.Events;

namespace Utils.Behaviours.BehaviourMessagesEvents
{
    public static class ObserverExtensionMethods
    {
        public static void RegisterOnAwake(this GameObject go, UnityAction action)
        {
            if (!go.TryGetComponent<AwakeMessage>(out var component))
            {
                component = go.AddComponent<AwakeMessage>();
            }
            
            component.OnAwakeEvent.AddListener(action);
        }
        
        public static void UnregisterOnAwake(this GameObject go, UnityAction action)
        {
            if (!go.TryGetComponent<AwakeMessage>(out var component))
            {
                component = go.AddComponent<AwakeMessage>();
            }
            
            component.OnAwakeEvent.RemoveListener(action);
        }
        
        public static void RegisterOnStart(this GameObject go, UnityAction action)
        {
            if (!go.TryGetComponent<StartMessage>(out var component))
            {
                component = go.AddComponent<StartMessage>();
            }
            
            component.OnStartEvent.AddListener(action);
        }
        
        public static void UnregisterOnStart(this GameObject go, UnityAction action)
        {
            if (!go.TryGetComponent<StartMessage>(out var component))
            {
                component = go.AddComponent<StartMessage>();
            }
            
            component.OnStartEvent.RemoveListener(action);
        }
        
        public static void RegisterOnEnable(this GameObject go, UnityAction action)
        {
            if (!go.TryGetComponent<OnEnableMessage>(out var component))
            {
                component = go.AddComponent<OnEnableMessage>();
            }
            
            component.OnEnableEvent.AddListener(action);
        }
        
        public static void UnregisterOnEnable(this GameObject go, UnityAction action)
        {
            if (!go.TryGetComponent<OnEnableMessage>(out var component))
            {
                component = go.AddComponent<OnEnableMessage>();
            }
            
            component.OnEnableEvent.RemoveListener(action);
        }
        
        public static void RegisterOnDisable(this GameObject go, UnityAction action)
        {
            if (!go.TryGetComponent<OnDisableMessage>(out var component))
            {
                component = go.AddComponent<OnDisableMessage>();
            }
            
            component.OnDisableEvent.AddListener(action);
        }
        
        public static void UnregisterOnDisable(this GameObject go, UnityAction action)
        {
            if (!go.TryGetComponent<OnDisableMessage>(out var component))
            {
                component = go.AddComponent<OnDisableMessage>();
            }
            
            component.OnDisableEvent.RemoveListener(action);
        }
        
        public static void RegisterOnDestroy(this GameObject go, UnityAction action)
        {
            if (!go.TryGetComponent<OnDestroyMessage>(out var component))
            {
                component = go.AddComponent<OnDestroyMessage>();
            }
            
            component.OnDestroyEvent.AddListener(action);
        }
        
        public static void UnregisterOnDestroy(this GameObject go, UnityAction action)
        {
            if (!go.TryGetComponent<OnDestroyMessage>(out var component))
            {
                component = go.AddComponent<OnDestroyMessage>();
            }
            
            component.OnDestroyEvent.RemoveListener(action);
        }
        
        public static void RegisterOnCollisionEnter(this GameObject go, UnityAction<Collision> action)
        {
            if (!go.TryGetComponent<OnCollisionEnterMessage>(out var component))
            {
                component = go.AddComponent<OnCollisionEnterMessage>();
            }
            
            component.OnCollisionEnterEvent.AddListener(action);
        }
        
        public static void UnregisterOnCollisionEnter(this GameObject go, UnityAction<Collision> action)
        {
            if (!go.TryGetComponent<OnCollisionEnterMessage>(out var component))
            {
                component = go.AddComponent<OnCollisionEnterMessage>();
            }
            
            component.OnCollisionEnterEvent.RemoveListener(action);
        }
        
        public static void RegisterOnCollisionExit(this GameObject go, UnityAction<Collision> action)
        {
            if (!go.TryGetComponent<OnCollisionExitMessage>(out var component))
            {
                component = go.AddComponent<OnCollisionExitMessage>();
            }
            
            component.OnCollisionExitEvent.AddListener(action);
        }
        
        public static void UnregisterOnCollisionExit(this GameObject go, UnityAction<Collision> action)
        {
            if (!go.TryGetComponent<OnCollisionExitMessage>(out var component))
            {
                component = go.AddComponent<OnCollisionExitMessage>();
            }
            
            component.OnCollisionExitEvent.RemoveListener(action);
        }
        
        public static void RegisterOnCollisionEnter2D(this GameObject go, UnityAction<Collision2D> action)
        {
            if (!go.TryGetComponent<OnCollisionEnter2DMessage>(out var component))
            {
                component = go.AddComponent<OnCollisionEnter2DMessage>();
            }
            
            component.OnCollisionEnter2DEvent.AddListener(action);
        }
        
        public static void UnregisterOnCollisionEnter2D(this GameObject go, UnityAction<Collision2D> action)
        {
            if (!go.TryGetComponent<OnCollisionEnter2DMessage>(out var component))
            {
                component = go.AddComponent<OnCollisionEnter2DMessage>();
            }
            
            component.OnCollisionEnter2DEvent.RemoveListener(action);
        }
        
        public static void RegisterOnCollisionExit2D(this GameObject go, UnityAction<Collision2D> action)
        {
            if (!go.TryGetComponent<OnCollisionExit2DMessage>(out var component))
            {
                component = go.AddComponent<OnCollisionExit2DMessage>();
            }
            
            component.OnCollisionExit2DEvent.AddListener(action);
        }
        
        public static void UnregisterOnCollisionExit2D(this GameObject go, UnityAction<Collision2D> action)
        {
            if (!go.TryGetComponent<OnCollisionExit2DMessage>(out var component))
            {
                component = go.AddComponent<OnCollisionExit2DMessage>();
            }
            
            component.OnCollisionExit2DEvent.RemoveListener(action);
        }
        
        public static void RegisterOnTriggerEnter(this GameObject go, UnityAction<Collider> action)
        {
            if (!go.TryGetComponent<OnTriggerEnterMessage>(out var component))
            {
                component = go.AddComponent<OnTriggerEnterMessage>();
            }
            
            component.OnTriggerEnterEvent.AddListener(action);
        }
        
        public static void UnregisterOnTriggerEnter(this GameObject go, UnityAction<Collider> action)
        {
            if (!go.TryGetComponent<OnTriggerEnterMessage>(out var component))
            {
                component = go.AddComponent<OnTriggerEnterMessage>();
            }
            
            component.OnTriggerEnterEvent.RemoveListener(action);
        }
        
        public static void RegisterOnTriggerExit(this GameObject go, UnityAction<Collider> action)
        {
            if (!go.TryGetComponent<OnTriggerExitMessage>(out var component))
            {
                component = go.AddComponent<OnTriggerExitMessage>();
            }
            
            component.OnTriggerExitEvent.AddListener(action);
        }
        
        public static void UnregisterOnTriggerExit(this GameObject go, UnityAction<Collider> action)
        {
            if (!go.TryGetComponent<OnTriggerExitMessage>(out var component))
            {
                component = go.AddComponent<OnTriggerExitMessage>();
            }
            
            component.OnTriggerExitEvent.RemoveListener(action);
        }
        
        public static void RegisterOnTriggerEnter2D(this GameObject go, UnityAction<Collider2D> action)
        {
            if (!go.TryGetComponent<OnTriggerEnter2DMessage>(out var component))
            {
                component = go.AddComponent<OnTriggerEnter2DMessage>();
            }
            
            component.OnTriggerEnter2DEvent.AddListener(action);
        }
        
        public static void UnregisterOnTriggerEnter2D(this GameObject go, UnityAction<Collider2D> action)
        {
            if (!go.TryGetComponent<OnTriggerEnter2DMessage>(out var component))
            {
                component = go.AddComponent<OnTriggerEnter2DMessage>();
            }
            
            component.OnTriggerEnter2DEvent.RemoveListener(action);
        }
        
        public static void RegisterOnTriggerExit2D(this GameObject go, UnityAction<Collider2D> action)
        {
            if (!go.TryGetComponent<OnTriggerExit2DMessage>(out var component))
            {
                component = go.AddComponent<OnTriggerExit2DMessage>();
            }
            
            component.OnTriggerExit2DEvent.AddListener(action);
        }
        
        public static void UnregisterOnTriggerExit2D(this GameObject go, UnityAction<Collider2D> action)
        {
            if (!go.TryGetComponent<OnTriggerExit2DMessage>(out var component))
            {
                component = go.AddComponent<OnTriggerExit2DMessage>();
            }
            
            component.OnTriggerExit2DEvent.RemoveListener(action);
        }
    }
}