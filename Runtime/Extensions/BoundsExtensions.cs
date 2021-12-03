using UnityEngine;

namespace Utils.Extensions
{
    public static class BoundsExtensions
    {
        public static Vector2 RandomPos(this Bounds bounds)
        {
            var min = bounds.min;
            var max = bounds.max;

            var pos = Vector2.zero;
            pos.x = Random.Range(min.x, max.x);
            pos.y = Random.Range(min.y, max.y);
        
            return pos;
        }
    }
}

