using System.Collections.Generic;
using UnityEngine;

namespace Utils.Runtime.Extensions
{
    public static class ListExtensions
    {
        public static T GetRandom<T>(List<T> list)
        {
            int count = list.Count;
            int index = Random.Range(0, count);
            return list[index];
        }
    }
}