﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using Vector3 = UnityEngine.Vector3;

namespace Utils.Tweening
{
    public static class TweeningUtils
    {
        public static async Task TweenTimeAsync(
            Action<float> timeDependantFunction,
            float duration,
            Curves.TimeCurveFunction timeCurve,
            CancellationToken ct)
        {
            timeDependantFunction(0);
            
            float startTime = Time.time;
            float timeToFinish = startTime + duration;
            while (Time.time <= timeToFinish && !ct.IsCancellationRequested)
            {
                float time = Time.time - startTime;
                float normalizedTime = timeCurve(time, duration);
                timeDependantFunction(normalizedTime);
                await Task.Yield();
            }
            
            timeDependantFunction(1);
        }

        public static async Task TweenFromToAsync(
            float from,
            float to,
            Action<float> setter,
            float duration,
            Curves.TimeCurveFunction easingCurve,
            CancellationToken ct)
        {
            var size = to - from;
            await TweenTimeAsync(val => setter(val * size + from), duration, easingCurve, ct);
        }
        
        public static IEnumerator TweenTimeCoroutine(
            Action<float> timeDependantFunction,
            float duration,
            Curves.TimeCurveFunction timeCurve)
        {
            timeDependantFunction(0);
            
            float startTime = Time.time;
            float timeToFinish = startTime + duration;
            while (Time.time <= timeToFinish)
            {
                float time = Time.time - startTime;
                float normalizedTime = timeCurve(time, duration);
                timeDependantFunction(normalizedTime);
                yield return null;
            }
            
            timeDependantFunction(1);
        }
        
        public static IEnumerator TweenFromToCoroutine(
            float from,
            float to,
            Action<float> setter,
            float duration,
            Curves.TimeCurveFunction easingCurve)
        {
            var size = to - from;
            return TweenTimeCoroutine(val => setter(val * size + from), duration, easingCurve);
        }
        
        public static async Task TweenMoveAsync(
            this Transform transform,
            Vector3 from,
            Vector3 to,
            float duration,
            Curves.TimeCurveFunction timeCurve,
            CancellationToken ct)
        {
            await TweenTimeAsync(normalizedTime =>
                {
                    var newPosition = Vector3.Lerp(from, to, normalizedTime);
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
            await TweenTimeAsync(normalizedTime =>
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
        
        public static IEnumerator TweenSpritesCoroutine(
            this SpriteRenderer spriteRenderer,
            IList<Sprite> sprites,
            float duration,
            Curves.TimeCurveFunction timeCurve)
        {
            if (sprites.Count > 0)
            {
                return TweenTimeCoroutine(normalizedTime =>
                    {
                        int index = (int) ((sprites.Count - 1) * normalizedTime);
                        var sprite = sprites[index];
                        spriteRenderer.sprite = sprite;
                    },
                    duration,
                    timeCurve
                );
            }

            return null;
        }
    }
}