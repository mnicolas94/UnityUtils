using System.Collections.Generic;
using UnityEngine;

namespace Utils.Extensions
{
    public static class RectExtensions
    {
        public static IEnumerable<float> HorizontalEvenlySplit(this Rect rect)
        {
            float log = Mathf.Log10(rect.width);
            int logFloor = Mathf.FloorToInt(log);
            log = log > logFloor + 0.5f ? logFloor : logFloor - 1;
            float increment = Mathf.Pow(10, log);
            float firstValue = Mathf.Ceil(rect.xMin / increment) * increment;
            for (float i = firstValue; i <= rect.xMax; i += increment)
            {
                yield return i;
            }
        }
        
        public static IEnumerable<float> VerticalEvenlySplit(this Rect rect)
        {
            float log = Mathf.Log10(rect.height);
            int logFloor = Mathf.FloorToInt(log);
            log = log > logFloor + 0.5f ? logFloor : logFloor - 1;
            float increment = Mathf.Pow(10, log);
            float firstValue = Mathf.Ceil(rect.yMin / increment) * increment;
            for (float i = firstValue; i <= rect.yMax; i += increment)
            {
                yield return i;
            }
        }
    }
}