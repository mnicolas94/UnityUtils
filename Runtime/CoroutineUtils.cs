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
        
        public static IEnumerator ActionCoroutine(Action action, bool afterFrame=true)
        {
            if (afterFrame)
                yield return null;
        
            action?.Invoke();
            
            if (!afterFrame)
                yield return null;
        }

        public static IEnumerator WaitTimeCoroutine(float time)
        {
            yield return new WaitForSeconds(time);
        }
    }
}