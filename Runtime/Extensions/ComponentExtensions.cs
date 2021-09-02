using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;

namespace Utils.Runtime.Extensions
{
    public static class ComponentExtensions
    {
        public static T CopyComponent<T>(this T source, GameObject destination) where T : Component
        {
            System.Type type = source.GetType();
            var dst = destination.AddComponent(type) as T;

            var fields = GetAllFields(type);
            foreach (var field in fields)
            {
                if (field.IsStatic) continue;
                field.SetValue(dst, field.GetValue(source));
            }

            var props = type.GetProperties();
            foreach (var prop in props)
            {
                if (!prop.CanRead || !prop.CanWrite || prop.Name == "name") continue;
                prop.SetValue(dst, prop.GetValue(source, null), null);
            }

            return dst;
        }

        public static IEnumerable<FieldInfo> GetAllFields(System.Type type)
        {
            if (type == null)
            {
                return Enumerable.Empty<FieldInfo>();
            }

            BindingFlags flags = BindingFlags.Public | BindingFlags.NonPublic |
                                 BindingFlags.Static | BindingFlags.Instance |
                                 BindingFlags.DeclaredOnly;
            return type.GetFields(flags).Concat(GetAllFields(type.BaseType));
        }
    }
}
