using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

namespace Crush2048
{
    public class PopupOpener : MonoBehaviour
    {
        [SerializeField] CanvasGroup UIToFade;
        [SerializeField] RectTransform objectToAnimate;
        [Range(0, 2f)] [SerializeField] float moveAmount;

        private Vector3 _initialPosition;
        private Vector3 _initialScale;
        private Vector3 _loweredScale;

        private const float ANIMATION_DURATION = 0.22f;

        private void Start()
        {
            _initialPosition = objectToAnimate.position;
            _initialScale = objectToAnimate.localScale;
            _loweredScale = _initialScale * 0.9f;
        }
       
        public void ShowPopup()
        {
            ActivateObjects();
            AnimatePopup();
        }

        private void ActivateObjects() => UIToFade.gameObject.SetActive(true);

        private void AnimatePopup()
        {
            UIToFade.alpha = 0;

            Vector3 startPosition = new Vector3(_initialPosition.x, _initialPosition.y - moveAmount, _initialPosition.z);
            Vector3 midPosition = new Vector3(_initialPosition.x, _initialPosition.y + (moveAmount / 2), _initialPosition.z);

            objectToAnimate.position = startPosition;
            objectToAnimate.localScale = _loweredScale;

            UIToFade.DOFade(1, ANIMATION_DURATION).SetEase(Ease.OutCirc);
            objectToAnimate.DOMove(midPosition, ANIMATION_DURATION / 2).OnComplete(() => objectToAnimate.DOMove(_initialPosition, ANIMATION_DURATION)).SetEase(Ease.OutCirc);
            objectToAnimate.DOScale(_initialScale, ANIMATION_DURATION).SetEase(Ease.OutCirc);
        }
    }
}
