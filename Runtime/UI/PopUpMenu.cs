using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

namespace Utils.UI
{
    public class PopUpMenu : MonoBehaviour
    {
        public UnityEvent eventBeforeShow;
        public UnityEvent eventBeforeHide;
        public UnityEvent eventShowed;
        public UnityEvent eventHidden;

        [SerializeField] private Transform menu;

        [SerializeField] private bool hideAtStart;
        [SerializeField] private Vector3 initPosition;
        [SerializeField] private Vector3 endPosition;
        [SerializeField] private bool relative;
        [SerializeField] private Transform relativeTo;
        
        [SerializeField] private float timeToShow;
        [SerializeField] private float timeToHide;

        [SerializeField] private InputAction cancelAction;
        [SerializeField] private bool cancellable;

        private bool _moving;

        public bool IsShowing { get; private set; }

        private void Start()
        {
            cancelAction.Enable();
            cancelAction.performed += context =>
            {
                if (cancellable)
                    HidePanel();
            };
            
            if (hideAtStart)
                HideImmediately();
        }

        private void OnEnable()
        {
            cancelAction?.Enable();
        }

        private void OnDisable()
        {
            cancelAction?.Disable();
        }

        [ContextMenu("ShowPanel")]
        public void ShowPanel()
        {
            bool alreadyShowing = menu.gameObject.activeSelf;
            if (_moving || alreadyShowing)
                return;
            menu.gameObject.SetActive(true);
            IsShowing = true;

            eventBeforeShow?.Invoke();
            StartCoroutine(MoveToTarget(
                initPosition,
                endPosition,
                timeToShow, () =>
                {
                    EventSystem.current.SetSelectedGameObject(gameObject);
                    eventShowed?.Invoke();
                }
            ));
        }
        
        [ContextMenu("HidePanel")]
        public void HidePanel()
        {
            bool alreadyHidden = !menu.gameObject.activeSelf;
            if (_moving || alreadyHidden)
                return;
            
            eventBeforeHide?.Invoke();
            StartCoroutine(MoveToTarget(
                endPosition, 
                initPosition, 
                timeToHide, () =>
                {
                    HideImmediately();
                    eventHidden?.Invoke();
                }
            ));
        }

        private void HideImmediately()
        {
            menu.localPosition = endPosition;
            menu.gameObject.SetActive(false);
            IsShowing = false;
        }
        
        public void HidePanel(float time)
        {
            Invoke(nameof(HidePanel), time);
        }

        private IEnumerator MoveToTarget(Vector3 fromPosition, Vector3 targetPosition, float time, Action atEnd)
        {
            _moving = true;

            // relative to camera
            if (relative)
            {
                var relativePosition = relativeTo.position;
                
                fromPosition.x += relativePosition.x;
                fromPosition.y += relativePosition.y;
                targetPosition.x += relativePosition.x;
                targetPosition.y += relativePosition.y;
            }
            
            float startTime = Time.time;
            float endTime = startTime + time;
            while (Time.time <= endTime)
            {
                float normTime = (Time.time - startTime) / time;
                var position = Vector3.Lerp(fromPosition, targetPosition, normTime);
                menu.localPosition = position;
                yield return null;
            }

            menu.localPosition = targetPosition;
            atEnd();
            
            _moving = false;
        }
    }
}