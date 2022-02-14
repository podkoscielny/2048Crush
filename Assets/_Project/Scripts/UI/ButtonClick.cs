using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using DG.Tweening;

namespace Crush2048
{
    public class ButtonClick : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
    {
        [SerializeField] List<RectTransform> objectsToBeScaled;

        private List<Vector3> _initialScalesOfObjects = new List<Vector3>(); 

        private const float LOWERED_SCALE_FACTOR = 0.9f;
        private const float TRANSITION_DURATION = 0.1f;

        private void Awake() => CacheObjectsScales();

        public void OnPointerDown(PointerEventData eventData)
        {
            for (int i = 0; i < objectsToBeScaled.Count; i++)
            {
                Vector3 scaleToBeSet = _initialScalesOfObjects[i] * LOWERED_SCALE_FACTOR;
                objectsToBeScaled[i].DOScale(scaleToBeSet, TRANSITION_DURATION);
            }
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            for (int i = 0; i < objectsToBeScaled.Count; i++)
            {
                Vector3 scaleToBeSet = _initialScalesOfObjects[i];
                objectsToBeScaled[i].DOScale(scaleToBeSet, TRANSITION_DURATION);
            }
        }

        private void CacheObjectsScales()
        {
            foreach (RectTransform rectTransform in objectsToBeScaled)
            {
                _initialScalesOfObjects.Add(rectTransform.localScale);
            }
        }
    }
}
