using System;
using System.Collections.Generic;
using UnityEngine;

namespace Utils.Runtime
{
    public static class VectorUtils
    {
        public static Vector2 Mean(IEnumerable<Vector2> vectors)
        {
            int len = 0;
            Vector2 sum = Vector2.zero;
            foreach (Vector2 v in vectors)
            {
                sum += v;
                len++;
            }
            return sum / len;
        }
    
        public static Vector2 FromAngle(float angle)
        {
            float rad = Mathf.Deg2Rad * angle;
            return new Vector2((float)Math.Cos(rad), (float)Math.Sin(rad));
        }
    }
}
