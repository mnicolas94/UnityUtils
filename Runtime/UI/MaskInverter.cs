using System;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;

namespace Common.UI
{
    /// <summary>
    /// credits to: https://github.com/dreamcodestudio/UIMask
    /// </summary>
    public sealed class MaskInverter : MonoBehaviour, IMaterialModifier
    {
        private static readonly int _stencilComp = Shader.PropertyToID("_StencilComp");

        public Material GetModifiedMaterial(Material baseMaterial)
        {
            var resultMaterial = new Material(baseMaterial);
            resultMaterial.SetFloat(_stencilComp, (int)CompareFunction.NotEqual);
            return resultMaterial;
        }
    }
}