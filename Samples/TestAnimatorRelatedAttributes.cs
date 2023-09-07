using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Utils.Attributes.Animations;

public class TestAnimatorRelatedAttributes : MonoBehaviour
{
    [SerializeField] private Animator _animator;

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
}
