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

        private Vector3 _punchScale = new Vector3(0.1f, 0.1f, 0.1f);

        private const float ANIMATION_DURATION = 0.4f;
       
        public void ShowPopup()
        {
            ActivateObjects();
            AnimatePopup();
        }

        private void ActivateObjects() => UIToFade.gameObject.SetActive(true);

        private void AnimatePopup()
        {
            UIToFade.alpha = 0;

            UIToFade.DOFade(1, ANIMATION_DURATION).SetEase(Ease.OutCirc);
            objectToAnimate.DOPunchScale(_punchScale, ANIMATION_DURATION).SetEase(Ease.OutCirc);
        }
    }
}
