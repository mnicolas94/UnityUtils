using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Utils
{
    public class PrefabsToInstanceMap
    {
        private Dictionary<Object, Object> _instances = new Dictionary<Object, Object>();

        public bool ExistsInstance(Object prefab)
        {
            return _instances.ContainsKey(prefab);
        }
        
        public Object GetOrCreateInstance(Object prefab)
        {
            if (!_instances.ContainsKey(prefab))
            {
                var instance = Object.Instantiate(prefab);
                _instances.Add(prefab, instance);

                return instance;
            }
            else
            {
                return _instances[prefab];
            }
        }

        public T GetOrCreateInstance<T>(Object prefab) where T : Object
        {
            return (T) GetOrCreateInstance(prefab);
        }

        public List<Object> GetAllInstances()
        {
            return _instances.Values.ToList();
        }

        public List<T> GetAllInstancesOfType<T>() where T : Object
        {
            return GetAllInstances().FindAll(instance => instance is T).ConvertAll(instance => (T) instance);
        }

        public void RemoveAndDestroyInstance(Object prefab)
        {
            if (_instances.ContainsKey(prefab))
            {
                var instance = _instances[prefab];
                _instances.Remove(prefab);
                Object.Destroy(instance);
            }
        }
    }
}