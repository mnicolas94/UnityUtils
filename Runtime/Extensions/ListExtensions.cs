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
        
        public static T PopRandom<T>(this IList<T> list)
        {
            var random = list.GetRandom();
            list.Remove(random);
            return random;
        }

        public static void AddRangeIfNotExists<T>(this List<T> list, IEnumerable<T> range)
        {
            foreach (var t in range)
            {
                if (!list.Contains(t))
                {
                    list.Add(t);
                }
            }
        }
        
        public static void AddRange<T>(this List<T> list, IEnumerable<T> range, int count)
        {
            var i = 0;
            foreach (var t in range)
            {
                if (i < count)
                {
                    list.Add(t);
                }
                else
                {
                    break;
                }

                i++;
            }
        }
    }
}