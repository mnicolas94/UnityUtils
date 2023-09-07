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
    }
}