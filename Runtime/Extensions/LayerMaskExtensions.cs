using UnityEngine;

namespace Utils.Extensions
{
    public static class LayerMaskExtensions
    {
        public static bool IsLayerInMask(this LayerMask mask, int layer)
        {
            return ((1 << layer) & mask.value) > 0;
        }
    }
}
