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
    }
}