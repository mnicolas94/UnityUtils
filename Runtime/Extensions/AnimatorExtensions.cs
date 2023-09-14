using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

namespace Utils.Extensions
{
    public static class AnimatorExtensions
    {
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
            while (!ct.IsCancellationRequested && !animator.IsCurrentState(layerIndex, stateHash))
            {
                await Task.Yield();
            }
        }
        
        public static async Task WaitUntilStateIsOverAsync(this Animator animator, int layerIndex, int stateHash, CancellationToken ct)
        {
            while (!ct.IsCancellationRequested && animator.IsCurrentState(layerIndex, stateHash))
            {
                await Task.Yield();
            }
        }
        
        public static async Task WaitUntilTransitionAsync(this Animator animator, int layerIndex, int transitionHash, CancellationToken ct)
        {
            while (!ct.IsCancellationRequested && !animator.IsCurrentTransition(layerIndex, transitionHash))
            {
                await Task.Yield();
            }
        }
        
        
        public static async Task WaitUntilTransitionIsAtAsync(this Animator animator, int layerIndex, int transitionHash, float percent, CancellationToken ct)
        {
            var isCurrent = animator.IsCurrentTransition(layerIndex, transitionHash);
            var reachedTime = false;
            var transitionIsCompleted = false;
            while (!ct.IsCancellationRequested && !reachedTime && !transitionIsCompleted)
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