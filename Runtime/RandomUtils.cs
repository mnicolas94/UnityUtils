using UnityEngine;

namespace Utils
{
    public static class RandomUtils
    {
        /// <summary>
        /// Random number generated from a normal distribution with the Marsaglia polar method.
        /// Values are clamped to the range [-1, 1]
        /// </summary>
        /// <returns></returns>
        public static float RandomNormalMarsaglia()
        {
            float u, v, S;

            do
            {
                u = 2.0f * Random.value - 1.0f;
                v = 2.0f * Random.value - 1.0f;
                S = u * u + v * v;
            } while (S >= 1.0f);

            float std = u * Mathf.Sqrt(-2.0f * Mathf.Log(S) / S);
            float mean = 0f;
            float sigma = 1 / 3.0f;
            return Mathf.Clamp(std * sigma + mean, -1, 1);
        }
    }
}
