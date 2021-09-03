using System;
using System.Numerics;
using UnityEngine;
using Vector2 = UnityEngine.Vector2;

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
        
        public static Vector2 MirrorVertically(this Vector2 vector, Rect bounds)
        {
            float verticalPercent = (vector.y - bounds.yMin) / bounds.height;
            float mirroredY = (1 - verticalPercent) * bounds.height + bounds.yMin;
            var mirrored = new Vector2(vector.x, mirroredY);
            return mirrored;
        }
        
        public static Vector2 MirrorHorizontally(this Vector2 vector, Rect bounds)
        {
            float horizontalPercent = (vector.x - bounds.xMin) / bounds.width;
            float mirroredX = (1 - horizontalPercent) * bounds.width + bounds.xMin;
            var mirrored = new Vector2(mirroredX, vector.y);
            return mirrored;
        }

        public static bool IsInsideRect(this Vector2 vector, Rect rect)
        {
            bool left = vector.x >= rect.xMin;
            bool right = vector.x <= rect.xMax;
            bool bottom = vector.y >= rect.yMin;
            bool top = vector.y <= rect.yMax;
            return left && right && bottom && top;
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