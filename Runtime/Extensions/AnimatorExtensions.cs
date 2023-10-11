﻿using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

namespace Utils.Extensions
{
    public static class AnimatorExtensions
    {
        public static void RestartToDefaultState(this Animator animator)
        {
            animator.Rebind();
            animator.Update(0f);
        }
    
        public static bool IsCurrentState(this Animator animator, int layerIndex, string state)
        {
            return animator.GetCurrentAnimatorStateInfo(layerIndex).IsName(state);
        }
        
        public static bool IsCurrentState(this Animator animator, int layerIndex, int stateHash)
        {
            var state = animator.GetCurrentAnimatorStateInfo(layerIndex);
            return state.shortNameHash == stateHash || state.fullPathHash == stateHash;
        }
        
        public static bool IsCurrentTransition(this Animator animator, int layerIndex, string transitionName)
        {
            if (animator.IsInTransition(layerIndex))
            {
                return animator.GetAnimatorTransitionInfo(layerIndex).IsName(transitionName);
            }

            return false;
        }
        
        public static bool IsCurrentTransition(this Animator animator, int layerIndex, int transitionHash)
        {
            if (animator.IsInTransition(layerIndex))
            {
                return animator.GetAnimatorTransitionInfo(layerIndex).nameHash == transitionHash;
            }

            return false;
        }

        public static async Task WaitUntilStateAsync(this Animator animator, int layerIndex, int stateHash, CancellationToken ct)
        {
            while (!ct.IsCancellationRequested && animator.isActiveAndEnabled && !animator.IsCurrentState(layerIndex, stateHash))
            {
                await Task.Yield();
            }
        }
        
        public static async Task WaitUntilStateIsOverAsync(this Animator animator, int layerIndex, int stateHash, CancellationToken ct)
        {
            while (!ct.IsCancellationRequested && animator.isActiveAndEnabled && animator.IsCurrentState(layerIndex, stateHash))
            {
                await Task.Yield();
            }
        }
        
        public static async Task WaitUntilCurrentStateIsOverAsync(this Animator animator, int layerIndex, CancellationToken ct)
        {
            var currentState = animator.GetCurrentAnimatorStateInfo(layerIndex).shortNameHash;
            while (!ct.IsCancellationRequested && animator.isActiveAndEnabled && animator.IsCurrentState(layerIndex, currentState))
            {
                await Task.Yield();
            }
        }

        public static async Task WaitUntilCurrentStateLoopsNTimes(this Animator animator, int layerIndex, int loops,
            CancellationToken ct)
        {
            var currentState = animator.GetCurrentAnimatorStateInfo(layerIndex);
            var doesLoop = currentState.loop;
            if (doesLoop)
            {
                var loopCount = 0;
                while (loopCount < loops && !ct.IsCancellationRequested)
                {
                    loopCount = (int) animator.GetCurrentAnimatorStateInfo(layerIndex).normalizedTime;
                    await Task.Yield();
                }
            }
            else
            {
                await animator.WaitUntilCurrentStateIsOverAsync(layerIndex, ct);
            }
        }
        
        public static async Task WaitUntilTransitionAsync(this Animator animator, int layerIndex, int transitionHash, CancellationToken ct)
        {
            while (!ct.IsCancellationRequested && animator.isActiveAndEnabled && !animator.IsCurrentTransition(layerIndex, transitionHash))
            {
                await Task.Yield();
            }
        }
        
        public static async Task WaitUntilTransitionIsAtAsync(this Animator animator, int layerIndex, int transitionHash, float percent, CancellationToken ct)
        {
            var isCurrent = animator.IsCurrentTransition(layerIndex, transitionHash);
            var reachedTime = false;
            var transitionIsCompleted = false;
            while (!ct.IsCancellationRequested && animator.isActiveAndEnabled && !reachedTime && !transitionIsCompleted)
            {
                var transition = animator.GetAnimatorTransitionInfo(layerIndex);
                var newIsCurrent = animator.IsCurrentTransition(layerIndex, transitionHash);
                transitionIsCompleted = isCurrent && !newIsCurrent;
                isCurrent = newIsCurrent;
                reachedTime = isCurrent && transition.normalizedTime >= percent;
                await Task.Yield();
            }
        }
    }
}