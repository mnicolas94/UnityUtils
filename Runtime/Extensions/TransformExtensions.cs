using UnityEngine;

namespace Utils.Runtime.Extensions
{
    public static class TransformExtensions
    {
        public static void LookAt2D(this Transform transform, Vector3 target)
        {
            Vector3 pos = transform.position;
            target.z = pos.z;
            transform.up = target - pos;
        }
    }
}
