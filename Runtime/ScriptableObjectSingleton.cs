using System;
using System.Linq;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Utils
{
    public class ScriptableObjectSingleton<T> : ScriptableObject where T : ScriptableObjectSingleton<T>
    {
        private static T _instance;
        public static T Instance
        {
            get
            {
#if UNITY_EDITOR
                if (IsNull)
                {
                    _instance = Load();
                    AddToPreloadedAssets(_instance);
                }
#endif
                return _instance;
            }
        }

        private void OnEnable()
        {
//            if (_instance != null) return;
            Debug.Log($"SOS.OnEnable: {name}; null: {_instance == null}");
            _instance = (T) this;
#if UNITY_EDITOR
            AddToPreloadedAssets(_instance);
#endif
        }

        private void OnDisable()
        {
            Debug.Log($"SOS.OnDisable: {name}; null: {_instance == null}");
            _instance = null;
        }

        private static bool IsNull => _instance == null;
        
#if UNITY_EDITOR
        private static T Load()
        {
            var assets = AssetDatabase.FindAssets($"t:{typeof(T).Name}");
            
            if (assets.Length == 0)
            {
                throw new Exception($"Could not load any singleton object of type {typeof(T)}");
            }
            else if (assets.Length > 1)
            {
                Debug.LogWarning($"Multiple instances of singleton type {typeof(T)} found");
            }

            string assetPath = AssetDatabase.GUIDToAssetPath(assets[0]);
            var asset = AssetDatabase.LoadAssetAtPath<T>(assetPath);

            return asset;
        }
        
        private static void AddToPreloadedAssets(Object asset)
        {
            // add to preloaded assets if not yet
            var preloadedAssets = PlayerSettings.GetPreloadedAssets().ToList();
            if (!preloadedAssets.Contains(asset))
            {
                preloadedAssets.Add(asset);
                PlayerSettings.SetPreloadedAssets(preloadedAssets.ToArray());
            }
        }
#endif
    }
}