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
        
        public void LogBool(bool message)
        {
            Debug.Log(message.ToString());
        }
        
        public void LogInt(int message)
        {
            Debug.Log(message.ToString());
        }
        
        public void LogFloat(float message)
        {
            Debug.Log(message.ToString());
        }
        
        public void LogObject(Object message)
        {
            Debug.Log(message);
        }
    }
}