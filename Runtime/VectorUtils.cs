using System;
using System.Collections.Generic;
using UnityEngine;

namespace Utils.Runtime
{
    public static class VectorUtils
    {
        
        /// <summary>
        /// Devuelve un vector ortogonal al pasado por parámetro y de magnitud igual a 1.
        /// </summary>
        /// <param name="v"></param>
        /// <returns></returns>
        public static Vector2 OrtoNormalized(this Vector2 v)
        {
            Vector2 result;
            if (v.y == 0)
            {
                result = new Vector2(0, 1);
            }
            else
            {
                result = new Vector2(1f, -v.x/v.y).normalized;
            }
    
            if (Single.IsNaN(result.x))
            {
                Debug.Log(v);
            }
    
            return result;
        }
    
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
