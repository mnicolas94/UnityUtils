using System;
using System.Linq;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Utils.Editor
{
    public class ScriptableObjectSingleton<T> : ScriptableObject where T : ScriptableObjectSingleton<T>
    {
        private static T _instance;
        public static T Instance
        {
            get
            {
                if (IsNull)
                {
                    _instance = Load();
                }
                return _instance;
            }
        }
        
        private static bool IsNull => _instance == null;

        private void OnEnable()
        {
            _instance = (T) this;
            OnEnableCallback();
        }

        private void OnDisable()
        {
            _instance = null;
            OnDisableCallback();
        }
        
        protected virtual void OnEnableCallback(){}
        protected virtual void OnDisableCallback(){}

        private static T Load()
        {
            var assets = AssetDatabase.FindAssets($"t:{typeof(T).Name}");
            
            if (assets.Length == 0)
            {
                Debug.LogWarning($"Could not load any singleton object of type {typeof(T)}");
                return null;
            }
            else if (assets.Length > 1)
            {
                Debug.LogWarning($"!!! Multiple instances of singleton type {typeof(T)} found");
            }

            string assetPath = AssetDatabase.GUIDToAssetPath(assets[0]);
            var asset = AssetDatabase.LoadAssetAtPath<T>(assetPath);

            return asset;
        }
    }
}