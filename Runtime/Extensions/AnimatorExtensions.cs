using System.Threading;
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

        public static async Task WaitUntilCurrentStateLoopsNTimes(this Animator animator, int layerIndex, float loops,
            CancellationToken ct)
        {
            var currentState = animator.GetCurrentAnimatorStateInfo(layerIndex);
            var doesLoop = currentState.loop;
            if (doesLoop)
            {
                var loopCount = 0f;
                while (loopCount < loops && !ct.IsCancellationRequested)
                {
                    loopCount = animator.GetCurrentAnimatorStateInfo(layerIndex).normalizedTime;
                    await Task.Yield();
                }
            }
            else
            {
                await animator.WaitUntilCurrentStateIsOverAsync(layerIndex, ct);
            }
        }
        
        public static async Task WaitUntilCurrentStateIsAtNormalizedTime(this Animator animator, int layerIndex, float time,
            CancellationToken ct)
        {
            var normTime = 0f;
            while (normTime < time && !ct.IsCancellationRequested)
            {
                normTime = animator.GetCurrentAnimatorStateInfo(layerIndex).normalizedTime;
                await Task.Yield();
            }
        }
        
        /// <summary>
        /// Wait for a state to be exited (played) N times. Loops of that state aren't counted.
        /// </summary>
        /// <param name="animator"></param>
        /// <param name="layerIndex"></param>
        /// <param name="stateHash"></param>
        /// <param name="playedCount"></param>
        /// <param name="ct"></param>
        public static async Task WaitUntilStateIsPlayedNTimes(this Animator animator, int layerIndex, int stateHash,
            int playedCount, CancellationToken ct)
        {
            // if entered during transition ignore it and wait for it to be over
            if (animator.IsInTransition(layerIndex))
            {
                await animator.WaitUntilCurrentTransitionIsOverAsync(layerIndex, ct);
            }
            
            // start counting
            var count = 0;
            while (count < playedCount && !ct.IsCancellationRequested)
            {
                if (!animator.IsInTransition(layerIndex))
                {
                    await animator.WaitUntilAnyTransitionAsync(layerIndex, ct);
                }
                
                var current = animator.GetCurrentAnimatorStateInfo(layerIndex);
                var leavingState = current.shortNameHash == stateHash || current.fullPathHash == stateHash;
                if (leavingState)
                {
                    count++;
                    await animator.WaitUntilCurrentTransitionIsOverAsync(layerIndex, ct);
                }
                else
                {
                    await Task.Yield();
                }
            }
        }
        
        public static async Task WaitUntilCurrentTransitionIsOverAsync(this Animator animator, int layerIndex, CancellationToken ct)
        {
            var lastTransitionHash = animator.IsInTransition(layerIndex) ? animator.GetAnimatorTransitionInfo(layerIndex).nameHash : 0;
            var sameTransition = true;
            while (!ct.IsCancellationRequested && animator.isActiveAndEnabled && animator.IsInTransition(layerIndex) && sameTransition)
            {
                var currentTransitionHash = animator.GetAnimatorTransitionInfo(layerIndex).nameHash;
                sameTransition = currentTransitionHash == lastTransitionHash;
                lastTransitionHash = currentTransitionHash;
                
                await Task.Yield();
            }
        }
        
        public static async Task WaitUntilAnyTransitionAsync(this Animator animator, int layerIndex, CancellationToken ct)
        {
            while (!ct.IsCancellationRequested && animator.isActiveAndEnabled && !animator.IsInTransition(layerIndex))
            {
                await Task.Yield();
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