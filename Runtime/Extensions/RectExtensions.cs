using System.Collections.Generic;
using UnityEngine;

namespace Utils.Extensions
{
    public static class RectExtensions
    {
        public static IEnumerable<float> HorizontalEvenlySplit(this Rect rect)
        {
            return MathUtils.SplitNicely(rect.xMin, rect.xMax);
        }
        
        public static IEnumerable<float> VerticalEvenlySplit(this Rect rect)
        {
            return MathUtils.SplitNicely(rect.yMin, rect.yMax);
        }

        public static Rect Intersection(this Rect self, Rect other)
        {
            float xMin = Mathf.Max(self.xMin, other.xMin);
            float yMin = Mathf.Max(self.yMin, other.yMin);
            float xMax = Mathf.Min(self.xMax, other.xMax);
            float yMax = Mathf.Min(self.yMax, other.yMax);

            float width = xMax - xMin;
            float height = yMax - yMin;

            return new Rect(xMin, yMin, width, height);
        }
    }
}