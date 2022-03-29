using System.Threading.Tasks;
using UnityEngine;
using Vector3 = UnityEngine.Vector3;

namespace Utils.Tweening
{
    public static class TweeningUtils
    {
        public delegate float CurveFunction(float time, float duration);
        
        public static async Task TweenMoveAsync(
            this Transform transform,
            Vector3 from,
            Vector3 to,
            float duration,
            CurveFunction curveFunction)
        {
            float startTime = Time.time;
            float timeToFinish = startTime + duration;
            Vector3 newPosition;
            while (Time.time <= timeToFinish)
            {
                float time = Time.time - startTime;
                float normalizedTime = curveFunction(time, duration);
                newPosition = Vector3.Lerp(from, to, normalizedTime);
                transform.position = newPosition;

                await Task.Yield();
            }

            transform.position = to;
        }
    }
}