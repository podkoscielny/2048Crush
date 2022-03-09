using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Crush2048
{
    public class ButtonSound : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
    {
        public static event Action OnButtonUp;
        public static event Action OnButtonDown;
        public static event Action OnButtonClick;

        public void OnPointerDown(PointerEventData eventData) => OnButtonDown?.Invoke();

        public void OnPointerUp(PointerEventData eventData) => OnButtonUp?.Invoke();

        private void OnMouseEnter() => OnButtonClick?.Invoke();
    }
}
