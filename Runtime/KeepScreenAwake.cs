using UnityEngine;

namespace Utils
{
    public class KeepScreenAwake : MonoBehaviour
    {
        void Start()
        {
            Screen.sleepTimeout = SleepTimeout.NeverSleep;
        }
    }
}
