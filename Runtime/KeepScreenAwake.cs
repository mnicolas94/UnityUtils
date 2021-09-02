using UnityEngine;

namespace Utils.Runtime
{
    public class KeepScreenAwake : MonoBehaviour
    {
        void Start()
        {
            Screen.sleepTimeout = SleepTimeout.NeverSleep;
        }
    }
}
