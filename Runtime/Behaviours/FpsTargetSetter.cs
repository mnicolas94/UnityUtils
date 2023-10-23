using UnityEngine;

namespace Utils.Behaviours
{
    public class FpsTargetSetter : MonoBehaviour
    {
        [SerializeField] private int _targetFps;

        private void Start()
        {
            SetFps(_targetFps);
        }

        public void SetFps(int targetFps)
        {
            QualitySettings.vSyncCount = 0;
            Application.targetFrameRate = targetFps;
        }
    }
}