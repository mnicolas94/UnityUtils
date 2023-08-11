using UnityEngine;

namespace Utils
{
    [CreateAssetMenu(fileName = "DebugLogger", menuName = "Facticus/Utils/DebugLogger", order = 0)]
    public class ScriptableDebugLogger : ScriptableObject
    {
        public void DebugMessage(string message)
        {
            Debug.Log(message);
        }
    }
}