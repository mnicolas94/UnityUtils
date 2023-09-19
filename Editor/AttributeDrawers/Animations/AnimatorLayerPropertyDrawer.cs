using System.Collections.Generic;
using UnityEditor;
using UnityEditor.Animations;
using UnityEngine;
using Utils.Attributes.Animations;

namespace Utils.Editor.AttributeDrawers.Animations
{
    [CustomPropertyDrawer(typeof(AnimatorLayerAttribute))]
    public class AnimatorLayerPropertyDrawer : PropertyDrawer
    {
        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            var animatorLayerAttribute = (AnimatorLayerAttribute) attribute;
            return AnimatorAttributesUtility.GetPropertyHeight(property, label, animatorLayerAttribute.AnimatorName);
        }

        public override void OnGUI(Rect rect, SerializedProperty property, GUIContent label)
        {
            var animatorLayerAttribute = (AnimatorLayerAttribute) attribute;
            var animatorName = animatorLayerAttribute.AnimatorName;
            
            AnimatorController animatorController = AnimatorAttributesUtility.GetAnimatorController(property, animatorName);

            List<(AnimatorControllerLayer, int)> animatorLayers = new List<(AnimatorControllerLayer, int)>();
            if (animatorController)
            {
                int layersCount = animatorController.layers.Length;
                for (int i = 0; i < layersCount; i++)
                {
                    AnimatorControllerLayer layer = animatorController.layers[i];
                    animatorLayers.Add((layer, i));
                }
            }
            
            AnimatorAttributesUtility.OnGUI(rect, property, label, animatorController, animatorLayers,
                GetInt, GetString);
        }

        private int GetInt((AnimatorControllerLayer, int) layerIndex)
        {
            return layerIndex.Item2;
        }

        private string GetString((AnimatorControllerLayer, int) layerIndex)
        {
            return layerIndex.Item1.name;
        }
    }
}