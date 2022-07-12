using System;
using System.Text.RegularExpressions;
using UnityEngine;

namespace Utils.Serializables
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
//                return tp.Assembly.GetName().Name + "|" + tp.FullName;
                return $"type: {{class: {tp.Name}, ns: {tp.Namespace}, asm: {tp.Assembly.GetName().Name}}}";
            }
            else
            {
                return null;
            }
        }

        public static Type UnHashType(string hash, Type baseType)
        {
            if (!string.IsNullOrEmpty(hash))
            {
                var classMatches = new Regex(@"class: (?<class>\w+)").Matches(hash);
                var namespaceMatches = new Regex(@"ns: (?<ns>(\w+.)*\w+)").Matches(hash);
                var assemblyMatches = new Regex(@"asm: (?<asm>(\w+.)*\w+)").Matches(hash);

                string className = "";
                string assemblyName = "";
                string typeName = "";

                bool correctlySerialized = classMatches.Count > 0 && namespaceMatches.Count > 0 && assemblyMatches.Count > 0;
                if (correctlySerialized)
                {
                    className = classMatches[0].Groups["class"].Value;
                    string namespaceName = namespaceMatches[0].Groups["ns"].Value;
                    assemblyName = assemblyMatches[0].Groups["asm"].Value;
                    typeName = $"{namespaceName}.{className}";
                }
//                var arr = hash.Split('|');
//                string assemblyName = arr.Length > 0 ? arr[0] : string.Empty;
//                string typeName = arr.Length > 1 ? arr[1] : string.Empty;
                var tp = TypeUtil.ParseType(assemblyName, typeName);

#if UNITY_EDITOR
                // try recover type
                if (tp == null)
                {
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