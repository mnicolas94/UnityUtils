using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

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

        public static int GetRandomWeightedIndex(IList<float> weights)
        {
            var accumulated = new List<float>(weights);
            float sum = accumulated[0];
            
            for (int i = 1; i < accumulated.Count; i++)
            {
                float accum = accumulated[i - 1] + accumulated[i];
                accumulated[i] = accum;
                sum += accum;
            }

            var normalized = accumulated.ConvertAll(val => val / sum);

            float randomValue = Random.value;
            for (int i = 0; i < normalized.Count; i++)
            {
                if (randomValue < normalized[i])
                {
                    return i;
                }
            }

            throw new ArithmeticException("This should not happen");
        }
    }
}
