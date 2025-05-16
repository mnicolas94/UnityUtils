using System;
using UnityEngine;

namespace Utils
{
    [Serializable]
    public class GlobalObjectIdRuntime
    {
        [SerializeField] private string _id;
        public string ID => _id;

        protected bool Equals(GlobalObjectIdRuntime other)
        {
            return _id == other._id;
        }

        public override bool Equals(object obj)
        {
            if (obj is null) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != GetType()) return false;
            return Equals((GlobalObjectIdRuntime)obj);
        }

        public override int GetHashCode()
        {
            return (_id != null ? _id.GetHashCode() : 0);
        }
    }
}