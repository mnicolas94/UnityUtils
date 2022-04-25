using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Utils.Editor.AutocompleteAttribute
{
    [CreateAssetMenu(fileName = "AutocompleteAttributeCacheData", menuName = "AutocompleteAttributeCacheData")]
    public class AutocompleteAttributeCacheData : ScriptableObject, ISerializationCallbackReceiver
    {
        [SerializeField] private List<HashedAutocompleteEntry> _serializableValues;
        private Dictionary<string, Dictionary<ObjectProperty, string>> _cache;

        public Dictionary<string, Dictionary<ObjectProperty, string>> Cache => _cache;

        public void OnBeforeSerialize()
        {
            _serializableValues ??= new List<HashedAutocompleteEntry>();
            _serializableValues.Clear();
            _cache ??= new Dictionary<string, Dictionary<ObjectProperty, string>>();
            foreach (var hash in _cache.Keys)
            {
                var dict = _cache[hash];
                foreach (var obj in dict.Keys)
                {
                    string value = dict[obj];
                    var entry = new HashedAutocompleteEntry
                    {
                        hash = hash,
                        target = obj,
                        value = value
                    };
                    _serializableValues.Add(entry);
                }
            }
        }

        public void OnAfterDeserialize()
        {
            _cache = new Dictionary<string, Dictionary<ObjectProperty, string>>();
            foreach (var (hash, target, value) in _serializableValues)
            {
                if (!_cache.ContainsKey(hash))
                {
                    _cache.Add(hash, new Dictionary<ObjectProperty, string>());
                }

                var dict = _cache[hash];
                dict.Add(target, value);
            }
        }
    }

    [Serializable]
    public struct HashedAutocompleteEntry
    {
        public string hash;
        public ObjectProperty target;
        public string value;

        public void Deconstruct(out string hash, out ObjectProperty target, out string value)
        {
            hash = this.hash;
            target = this.target;
            value = this.value;
        }
    }

    [Serializable]
    public class ObjectProperty
    {
        [SerializeField] private Object _object;
        [SerializeField] private int _objectHash;
        [SerializeField] private string _propertyPath;

        public Object Obj => _object;

        public string PropertyPath => _propertyPath;

        public ObjectProperty(Object obj, string propertyPath)
        {
            _object = obj;
            _objectHash = obj.GetHashCode();
            _propertyPath = propertyPath;
        }

        public bool IsNull()
        {
            bool isNull = Obj == null;
            if (!isNull)
            {
                var so = new SerializedObject(Obj);
                var sp = so.FindProperty(PropertyPath);
                isNull = sp == null;
            }

            return isNull;
        }
        
        protected bool Equals(ObjectProperty other)
        {
            return Equals(_object, other._object) && _propertyPath == other._propertyPath;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((ObjectProperty) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return (_objectHash * 397) ^ (_propertyPath != null ? _propertyPath.GetHashCode() : 0);
            }
        }
    }
}