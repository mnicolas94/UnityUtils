using UnityEngine;

namespace Utils.Behaviours
{
    public class DevelopmentBuildDependant : MonoBehaviour
    {
        [SerializeField] private bool existsIfDevBuild;
        [SerializeField] private bool _onlyOnBuilds;
        
        private void Awake()
        {
            var shouldDestroy = Debug.isDebugBuild != existsIfDevBuild;
            shouldDestroy |= _onlyOnBuilds && Application.isEditor;
            if (shouldDestroy)
            {
                Destroy(gameObject);
            }
        }
    }
}