using System;
using UnityEngine;

namespace Utils.Runtime.Extensions
{
    public static class Vector3IntExtensions
    {
        public static int ManhattanDistance(this Vector3Int a, Vector3Int b)
        {
            var diff = a - b;
            int distance = Math.Abs(diff.x) + Math.Abs(diff.y);
            return distance;
        }
    }
}