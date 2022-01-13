using System.Collections.Generic;
using UnityEngine;

namespace Utils.Extensions
{
    public static class ListExtensions
    {
        public static T GetRandom<T>(this IList<T> list)
        {
            int count = list.Count;
            int index = Random.Range(0, count);
            return list[index];
        }

        public static void AddRangeIfNotExists<T>(this List<T> list, IList<T> range)
        {
            for (int i = 0; i < range.Count; i++)
            {
                var t = range[i];
                if (!list.Contains(t))
                {
                    list.Add(t);
                }
            }
        }
    }
}