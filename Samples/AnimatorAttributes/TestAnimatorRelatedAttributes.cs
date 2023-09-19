using UnityEngine;
using UnityEngine.Assertions;
using Utils.Attributes.Animations;

namespace Utils.Samples
{
    public class TestAnimatorRelatedAttributes : MonoBehaviour
    {
        [SerializeField] private Animator _animator;

        [Header("Parameters")]
        [SerializeField, AnimatorParam(nameof(_animator))] private string _animatorParamString;
        [SerializeField, AnimatorParam(nameof(_animator))] private int _animatorParamHash;
        [SerializeField, AnimatorParam(nameof(_animator), AnimatorControllerParameterType.Float)]
        private int _animatorParamFloat;
        [SerializeField, AnimatorParam(nameof(_animator), AnimatorControllerParameterType.Int)]
        private int _animatorParamInt;
        [SerializeField, AnimatorParam(nameof(_animator), AnimatorControllerParameterType.Bool)]
        private int _animatorParamBool;
        [SerializeField, AnimatorParam(nameof(_animator), AnimatorControllerParameterType.Trigger)]
        private int _animatorParamTrigger;
    
        [Header("Layers")]
        [SerializeField, AnimatorLayer(nameof(_animator))] private string _animatorLayerString;
        [SerializeField, AnimatorLayer(nameof(_animator))] private int _animatorLayerInt;

        [Header("States")]
        [SerializeField, AnimatorState(nameof(_animator))] private string _animatorStateString;
        [SerializeField, AnimatorState(nameof(_animator))] private int _animatorStateInt;
        
        [Header("Transitions")]
        [SerializeField, AnimatorTransition(nameof(_animator))] private string _animatorTransitionString;
        [SerializeField, AnimatorTransition(nameof(_animator))] private int _animatorTransitionInt;
        
        [ContextMenu("Test")]
        public void Test()
        {
            _animator.SetFloat(_animatorParamFloat, Random.value);
            _animator.SetInteger(_animatorParamInt, Random.Range(0, 100000));
            _animator.SetBool(_animatorParamBool, _animator.GetBool(_animatorParamBool));
            _animator.SetTrigger(_animatorParamTrigger);
            Assert.AreEqual(_animator.GetLayerIndex(_animatorLayerString), _animatorLayerInt);
        }
    }
}
