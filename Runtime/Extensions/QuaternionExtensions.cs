using UnityEngine;

namespace Utils.Extensions
{
    public static class QuaternionExtensions
    {
        /// <summary>
        /// Cambiar un quaternion de mano derecha a mano izquierda, o al revés.
        /// </summary>
        /// <param name="q"></param>
        /// <returns></returns>
        public static Quaternion SwapLeftRightCoordinateSystem(this Quaternion q)
        {
            return new Quaternion(q.x, q.y, -q.z, -q.w);
        }
    }
}
