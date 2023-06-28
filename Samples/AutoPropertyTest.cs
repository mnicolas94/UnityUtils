using System;
using Samples.FixSerializedReferences;
using UnityEngine;
using Utils.Attributes;
using Object = UnityEngine.Object;

namespace Samples
{
    public class AutoPropertyTest : MonoBehaviour
    {
        [SerializeField, AutoProperty] private Rigidbody _rigidbody;
        [SerializeField, AutoProperty(AutoPropertyMode.Parent)] private AudioSource _audioSource;
        [SerializeField, AutoProperty(AutoPropertyMode.Scene)] private Camera _camera;
        [SerializeField, AutoProperty(AutoPropertyMode.Asset)] private ScriptableObjectReference _sor;
        [SerializeField] private CustomClass _customClass;
    }

    [Serializable]
    public class CustomClass
    {
        [SerializeField, AutoProperty(AutoPropertyMode.Asset, nameof(Predicate), typeof(CustomClass))]
        private ScriptableObjectReference _sorPredicate;

        public bool Predicate(Object asset)
        {
            return true;
        }
    }
}