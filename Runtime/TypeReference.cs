using System;
using UnityEngine;

namespace Utils
{
    /**
     * Taken mostly from
     * https://github.com/lordofduct/spacepuppy-unity-framework-4.0/blob/master/Framework/com.spacepuppy.core/Runtime/src/TypeReference.cs
     */
    [Serializable]
    public abstract class TypeReference
    {
        [SerializeField] private string _typeHash;
        [NonSerialized] private Type _type;
        
        public Type Type
        {
            get
            {
                if (_type == null) _type = UnHashType(_typeHash, GetBaseType());
                return _type;
            }
            set
            {
                _type = value;
                _typeHash = HashType(_type);
            }
        }

        public static implicit operator Type(TypeReference a)
        {
            return a.Type;
        }

        public abstract Type GetBaseType();
        
        public static string HashType(Type tp)
        {
            if (tp != null)
            {
                return tp.Assembly.GetName().Name + "|" + tp.FullName;
            }
            else
            {
                return null;
            }
        }

        public Type UnHashType(string hash, Type baseType)
        {
            if (!string.IsNullOrEmpty(hash))
            {
                var arr = hash.Split('|');
                string assemblyName = arr.Length > 0 ? arr[0] : string.Empty;
                string typeName = arr.Length > 1 ? arr[1] : string.Empty;
                var tp = TypeUtil.ParseType(assemblyName, typeName);

#if UNITY_EDITOR
                // try recover type
                if (tp == null)
                {
                    var tokens = typeName.Split('.');
                    string className = tokens[tokens.Length - 1];
                    tp = TypeUtil.GetSubclassTypeByName(baseType, className);
                }
#endif

                //set type to void if the type is unfruitful, this way we're not constantly retesting this
                if (tp == null)
                {   
                    tp = typeof(void);
                }
                
                return tp;
            }
            else
            {
                return null;
            }
        }
    }

    [Serializable]
    public class TypeReference<T> : TypeReference
    {
        public override Type GetBaseType()
        {
            return typeof(T);
        }
    }
}