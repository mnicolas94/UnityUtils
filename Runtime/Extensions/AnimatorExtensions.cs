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
            while (!animator.IsCurrentState(layerIndex, stateHash) && !ct.IsCancellationRequested)
            {
                await Task.Yield();
            }
        }
        
        public static async Task WaitUntilStateIsOverAsync(this Animator animator, int layerIndex, int stateHash, CancellationToken ct)
        {
            while (animator.IsCurrentState(layerIndex, stateHash) && !ct.IsCancellationRequested)
            {
                await Task.Yield();
            }
        }
        
        public static async Task WaitUntilTransitionAsync(this Animator animator, int layerIndex, int transitionHash, CancellationToken ct)
        {
            while (!animator.IsCurrentTransition(layerIndex, transitionHash) && !ct.IsCancellationRequested)
            {
                await Task.Yield();
            }
        }
    }
}