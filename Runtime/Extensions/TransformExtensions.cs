using UnityEngine;

namespace Utils.Extensions
{
    public static class TransformExtensions
    {
        public static void LookAt2D(this Transform transform, Vector3 target)
        {
            Vector3 pos = transform.position;
            target.z = pos.z;
            transform.up = target - pos;
        }
        
        public static void ClearChildren(this Transform transform)
        {
            while (transform.childCount > 0)
            {
                var child = transform.GetChild(0);
                if (Application.isPlaying)
                {
                    child.SetParent(null); // to avoid infinite loop
                    Object.Destroy(child.gameObject);
                }
                else
                {
                    Object.DestroyImmediate(child.gameObject);
                }
            }
        }
    }
}
