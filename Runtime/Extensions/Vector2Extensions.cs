using System;
using UnityEngine;

namespace Utils.Runtime.Extensions
{
    public static class Vector2Extensions
    {
        public static Vector2 RemapBounds(this Vector2 vector, Rect currentBounds, Rect targetBounds)
        {
            float xMin = currentBounds.xMin;
            float xMax = currentBounds.xMax;
            float yMin = currentBounds.yMin;
            float yMax = currentBounds.yMax;
            float width = xMax - xMin;
            float height = yMax - yMin;
            
            float targetXMin = targetBounds.xMin;
            float targetXMax = targetBounds.xMax;
            float targetYMin = targetBounds.yMin;
            float targetYMax = targetBounds.yMax;
            float targetWidth = targetXMax - targetXMin;
            float targetHeight = targetYMax - targetYMin;

            float x = (vector.x - xMin) / width * targetWidth + targetXMin;
            float y = (vector.y - yMin) / height * targetHeight + targetYMin;
            return new Vector2(x, y);
        }
        
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
    
            return result;
        }
    }
}