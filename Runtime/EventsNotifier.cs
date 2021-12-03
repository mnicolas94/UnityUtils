using System;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Utils
{
    public class EventsNotifier : MonoBehaviour,
        IPointerEnterHandler, IPointerDownHandler, IPointerClickHandler, IPointerUpHandler, IPointerExitHandler,
        IBeginDragHandler, IDragHandler, IEndDragHandler
    {
        public Action<PointerEventData> eventPointerEnter;
        public Action<PointerEventData> eventPointerDown;
        public Action<PointerEventData> eventPointerClick;
        public Action<PointerEventData> eventPointerUp;
        public Action<PointerEventData> eventPointerExit;
        public Action<PointerEventData> eventBeginDrag;
        public Action<PointerEventData> eventDrag;
        public Action<PointerEventData> eventEndDrag;
        
        public void OnPointerEnter(PointerEventData eventData)
        {
            eventPointerEnter?.Invoke(eventData);
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            eventPointerDown?.Invoke(eventData);
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            eventPointerClick?.Invoke(eventData);
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            eventPointerUp?.Invoke(eventData);
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            eventPointerExit?.Invoke(eventData);
        }

        public void OnBeginDrag(PointerEventData eventData)
        {
            eventBeginDrag?.Invoke(eventData);
        }

        public void OnDrag(PointerEventData eventData)
        {
            eventDrag?.Invoke(eventData);
        }

        public void OnEndDrag(PointerEventData eventData)
        {
            eventEndDrag?.Invoke(eventData);
        }
    }
}
