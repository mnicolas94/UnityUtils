using UnityEngine;

namespace Utils.Behaviours
{
    public class KeepScreenAwake : MonoBehaviour
    {
        void Start()
        {
            Screen.sleepTimeout = SleepTimeout.NeverSleep;
        }
    }
}
