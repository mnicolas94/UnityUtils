using System.Collections.Generic;
using UnityEngine;

namespace Utils.Samples.PrefabToInstancesMap
{
    public class PrefabsToInstanceTest : MonoBehaviour
    {
        [SerializeField] private List<GameObject> _goPrefabs;
        [SerializeField] private List<Transform> _transformPrefabs;
        
        private PrefabsToInstanceMap _map = new PrefabsToInstanceMap();

        [ContextMenu(nameof(InstantiateAllGos))]
        public void InstantiateAllGos()
        {
            foreach (var goPrefab in _goPrefabs)
            {
                _map.GetOrCreateInstance<GameObject>(goPrefab);
            }
        }

        [ContextMenu(nameof(RemoveAllGos))]
        public void RemoveAllGos()
        {
            foreach (var goPrefab in _goPrefabs)
            {
                _map.RemoveAndDestroyInstance(goPrefab);
            }
        }
        
        [ContextMenu(nameof(EnableAllGos))]
        public void EnableAllGos()
        {
            foreach (var goPrefab in _goPrefabs)
            {
                var inst = _map.GetOrCreateInstance<GameObject>(goPrefab);
                inst.SetActive(true);
            }
        }
        
        [ContextMenu(nameof(DisableAllGos))]
        public void DisableAllGos()
        {
            foreach (var goPrefab in _goPrefabs)
            {
                var inst = _map.GetOrCreateInstance<GameObject>(goPrefab);
                inst.SetActive(false);
            }
        }
        
        [ContextMenu(nameof(InstantiateAllTransforms))]
        public void InstantiateAllTransforms()
        {
            foreach (var goPrefab in _transformPrefabs)
            {
                _map.GetOrCreateInstance<Transform>(goPrefab);
            }
        }

        [ContextMenu(nameof(RemoveAllTransforms))]
        public void RemoveAllTransforms()
        {
            foreach (var goPrefab in _transformPrefabs)
            {
                _map.RemoveAndDestroyInstance(goPrefab);
            }
        }
        
        [ContextMenu(nameof(EnableAllTransforms))]
        public void EnableAllTransforms()
        {
            foreach (var goPrefab in _transformPrefabs)
            {
                var inst = _map.GetOrCreateInstance<Transform>(goPrefab);
                inst.gameObject.SetActive(true);
            }
        }
        
        [ContextMenu(nameof(DisableAllTransforms))]
        public void DisableAllTransforms()
        {
            foreach (var goPrefab in _transformPrefabs)
            {
                var inst = _map.GetOrCreateInstance<Transform>(goPrefab);
                inst.gameObject.SetActive(false);
            }
        }
    }
}