using UnityEngine;
using Vector3 = UnityEngine.Vector3;

namespace Utils.Runtime.Extensions
{
    public static class CollisionExtensions
    {
        public static Vector3 MeanContactPointsPosition(this Collision collision)
        {
            Vector3 sum = Vector3.zero;
            if (collision.contactCount > 0)
            {
                for (int i = 0; i < collision.contactCount; i++)
                {
                    sum += collision.GetContact(i).point;
                }

                sum /= collision.contactCount;    
            }
        
            return sum;
        }
    }    
}
