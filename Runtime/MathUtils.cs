using System.Collections.Generic;
using UnityEngine;

namespace Utils
{
    public static class MathUtils
    {
        public static IEnumerable<float> SplitNicely(float min, float max)
        {
            float width = max - min;
            float log = Mathf.Log10(width);
            int logFloor = Mathf.FloorToInt(log);
            float decimalValue = log - logFloor;
            float increment;

            if (decimalValue < 0.22f)
                increment = Mathf.Pow(10, logFloor - 1);
            else if (decimalValue < 0.82f)
                increment = Mathf.Pow(10, logFloor - 1) * 5;
            else
                increment = Mathf.Pow(10, logFloor);
            
            float firstValue = Mathf.Ceil(min / increment) * increment;
            for (float i = firstValue; i <= max; i += increment)
            {
                yield return i;
            }
        }

        public static Vector2 FromPolar(float magnitude, float angle)
        {
            var rad = angle * Mathf.Deg2Rad;
            float x = magnitude * Mathf.Cos(rad);
            float y = magnitude * Mathf.Sin(rad);
            return new Vector2(x, y);
        }
        
        public static float Normalize(float value, float rangeMin, float rangeMax)
        {
            var isInverted = rangeMin > rangeMax;
            var min = isInverted ? rangeMax : rangeMin;
            var max = isInverted ? rangeMin : rangeMax;
            return (value - min) / (max - min);
        }
    }
}