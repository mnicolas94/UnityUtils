using UnityEngine;

namespace Utils
{
    public class DevelopmentBuildDependant : MonoBehaviour
    {
        [SerializeField] private bool existsIfDevBuild;
        
        private void Awake()
        {
            if (Debug.isDebugBuild != existsIfDevBuild)
            {
                Destroy(gameObject);
            }
        }
    }
}