using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Utils
{
    public static class CoroutineUtils
    {
        public static IEnumerator CoroutineSequence(List<IEnumerator> coroutines)
        {
            foreach (var coroutine in coroutines)
            {
                yield return coroutine;
            }
        }
        
        public static IEnumerator ActionCoroutine(
            Action action,
            bool waitPreviousFrame=true,
            bool waitNextFrame=true)
        {
            if (waitPreviousFrame)
                yield return null;
        
            action?.Invoke();
            
            if (waitNextFrame)
                yield return null;
        }

        public static IEnumerator WaitTimeCoroutine(float time)
        {
            yield return new WaitForSeconds(time);
        }
        
        public static IEnumerator WaitCoroutine(MonoBehaviour owner, IEnumerator coroutine)
        {
            bool running = true;
            owner.StartCoroutine(CoroutineSequence(new List<IEnumerator>
            {
                coroutine,
                ActionCoroutine(() => running = false, false, false)
            }));

            while (running)
            {
                yield return null;
            }
        }
        
        public static IEnumerator WaitAll(MonoBehaviour owner, List<IEnumerator> coroutines)
        {
            int running = coroutines.Count;
            foreach (var coroutine in coroutines)
            {
                owner.StartCoroutine(CoroutineSequence(new List<IEnumerator>
                {
                    coroutine,
                    ActionCoroutine(() => running--, false, false)
                }));
            }

            while (running > 0)
            {
                yield return null;
            }
        }
    }
}