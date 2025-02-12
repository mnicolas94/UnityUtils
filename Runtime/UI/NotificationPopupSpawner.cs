using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

namespace Utils.UI
{
    [CreateAssetMenu(fileName = "NotificationSpawner", menuName = "Facticus/Utils/NotificationSpawner", order = 0)]
    public class NotificationPopupSpawner : ScriptableObject
    {
        [SerializeField] private float _notificationTime;
        [SerializeField] private NotificationPopup _notificationPrefab;

        public NotificationPopup NotificationPrefab
        {
            get => _notificationPrefab;
            set => _notificationPrefab = value;
        }
#if !UNITY_2022_1_OR_NEWER
        private CancellationTokenSource _cts;

        private void OnEnable()
        {
            _cts = new CancellationTokenSource();
        }

        private void OnDisable()
        {
            if (!_cts.IsCancellationRequested)
            {
                _cts.Cancel();
            }

            _cts.Dispose();
            _cts = null;
        }
#endif
        
        private List<NotificationPopup> _currentInstances = new();

        public void ClearNotifications()
        {
            while (_currentInstances.Count > 0)
            {
                DestroyInstance(_currentInstances[0]);
            }
        }

        public void SpawnNotification(string text)
        {
            ClearNotifications();
            SpawnNotificationNoClear(text);
        }

        public async void SpawnNotificationNoClear(string text)
        {
            var instance = Instantiate(_notificationPrefab);
            instance.SetText(text);
            // mark as dont-destroy-on-load in runtime only
            if (Application.isPlaying)
            {
                DontDestroyOnLoad(instance);
            }
            _currentInstances.Add(instance);

            await WaitNotificationTime(instance);

            DestroyInstance(instance);
        }

        private async Task WaitNotificationTime(NotificationPopup instance)
        {
            var start = Time.realtimeSinceStartup;
            var elapsedTime = 0f;
            var isInstanceAlive = _currentInstances.Contains(instance);
#if UNITY_2022_1_OR_NEWER
            var ct = Application.exitCancellationToken;
#else
            var ct = _cts.Token;
#endif            
            while (elapsedTime < _notificationTime
                   && isInstanceAlive
                   && !ct.IsCancellationRequested)
            {
                elapsedTime = Time.realtimeSinceStartup - start;
                isInstanceAlive = _currentInstances.Contains(instance);
                await Task.Yield();
            }
        }

        private void DestroyInstance(NotificationPopup instance)
        {
            if (!_currentInstances.Contains(instance))
            {
                return;
            }
            
            _currentInstances.Remove(instance);
            if (Application.isPlaying)
            {
                Destroy(instance.gameObject);
            }
            else
            {
                DestroyImmediate(instance.gameObject);
            }
        }
    }
}