using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using Vector3 = UnityEngine.Vector3;

namespace Utils.Tweening
{
    public static class TweeningUtils
    {
        public static async Task TweenTime(
            Action<float> timeDependantFunction,
            float duration,
            Curves.TimeCurveFunction timeCurve,
            CancellationToken ct)
        {
            float startTime = Time.time;
            float timeToFinish = startTime + duration;
            while (Time.time <= timeToFinish && !ct.IsCancellationRequested)
            {
                float time = Time.time - startTime;
                float normalizedTime = timeCurve(time, duration);
                timeDependantFunction(normalizedTime);
                await Task.Yield();
            }
        }
        
        public static async Task TweenMoveAsync(
            this Transform transform,
            Vector3 from,
            Vector3 to,
            float duration,
            Curves.TimeCurveFunction timeCurve,
            CancellationToken ct)
        {
            Vector3 newPosition;
            await TweenTime(normalizedTime =>
                {
                    newPosition = Vector3.Lerp(from, to, normalizedTime);
                    transform.position = newPosition;
                },
                duration,
                timeCurve,
                ct
            );

            transform.position = to;
        }

        public static async Task TweenSpritesAsync(
            this SpriteRenderer spriteRenderer,
            IList<Sprite> sprites,
            float duration,
            Curves.TimeCurveFunction timeCurve,
            CancellationToken ct)
        {
            await TweenTime(normalizedTime =>
                {
                    int index = (int)(sprites.Count * normalizedTime);
                    var sprite = sprites[index];
                    spriteRenderer.sprite = sprite;
                },
                duration,
                timeCurve,
                ct
            );
        }
    }
}