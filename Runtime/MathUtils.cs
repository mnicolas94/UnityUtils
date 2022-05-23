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
    }
}