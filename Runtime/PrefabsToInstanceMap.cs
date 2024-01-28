using System;
using System.Collections.Generic;
using System.Linq;
using Object = UnityEngine.Object;

namespace Utils
{
    public class PrefabsToInstanceMap
    {
        private Dictionary<Object, Object> _instances = new Dictionary<Object, Object>();

        private Action<Object> _onCreate;

        public Action<Object> OnCreate
        {
            get => _onCreate;
            set => _onCreate = value;
        }

        public bool ExistsInstance(Object prefab)
        {
            return _instances.ContainsKey(prefab);
        }
        
        public Object GetOrCreateInstance(Object prefab, Action<Object> onCreate)
        {
            if (!_instances.ContainsKey(prefab))
            {
                var instance = Object.Instantiate(prefab);
                _instances.Add(prefab, instance);
                
                onCreate.Invoke(instance);

                return instance;
            }
            else
            {
                return _instances[prefab];
            }
        }

        public T GetOrCreateInstance<T>(Object prefab, Action<Object> onCreate) where T : Object
        {
            return (T) GetOrCreateInstance(prefab, onCreate);
        }

        public List<Object> GetAllInstances()
        {
            return _instances.Values.ToList();
        }

        public List<T> GetAllInstancesOfType<T>() where T : Object
        {
            return GetAllInstances().FindAll(instance => instance is T).ConvertAll(instance => (T) instance);
        }
        
        public List<Object> GetAllPrefabs()
        {
            return _instances.Keys.ToList();
        }

        public List<T> GetAllPrefabsOfType<T>() where T : Object
        {
            return GetAllPrefabs().FindAll(prefab => prefab is T).ConvertAll(prefab => (T) prefab);
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