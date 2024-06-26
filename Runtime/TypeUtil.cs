﻿﻿using System;
using System.Collections.Generic;
using System.Linq;
 using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

 namespace Utils
{
    public static class TypeUtil
    {
        /**
         * Taken from https://github.com/lordofduct/spacepuppy-unity-framework-4.0/blob/master/Framework/com.spacepuppy.core/Runtime/src/Utils/TypeUtil.cs
         */
        public static Type ParseType(string assemblyName, string typeName)
        {
            var assemb = (from a in AppDomain.CurrentDomain.GetAssemblies()
                          where a.GetName().Name == assemblyName || a.FullName == assemblyName
                          select a).FirstOrDefault();
            if (assemb != null)
            {
                return (from t in assemb.GetTypes()
                        where t.FullName == typeName
                        select t).FirstOrDefault();
            }
            else
            {
                return null;
            }
        }

#if UNITY_EDITOR
        public static List<Type> GetSubclassTypes(Type baseType, bool allowInterfaces = false, bool allowAbstract = false)
        {
            bool Predicate(Type t)
            {
                return (!t.IsInterface || allowInterfaces ) &&
                       (!t.IsAbstract || allowAbstract ) &&
                       !t.IsPointer;
            }
            var types = TypeCache.GetTypesDerivedFrom(baseType).Where(Predicate).ToList();
            return types;
        }

        public static Type GetSubclassTypeByName(Type baseClass, string subClassName, bool allowInterfaces = false, bool allowAbstract = false)
        {
            bool Predicate(Type t)
            {
                return (!t.IsInterface || allowInterfaces ) &&
                       (!t.IsAbstract || allowAbstract ) &&
                       !t.IsPointer &&
                       t.Name == subClassName;
            }

            var exists = TypeCache.GetTypesDerivedFrom(baseClass).Any(Predicate);
            if (exists)
            {
                var type = TypeCache.GetTypesDerivedFrom(baseClass).First(Predicate);
                return type;
            }
            return null;
        }
#endif
    }
}
