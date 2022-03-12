using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

namespace Crush2048
{
    public class TitleImages : MonoBehaviour
    {
        [SerializeField] Transform imagesCenter;
        [SerializeField] List<Transform> images;

        private Vector3 _initialPosition;
        private Transform _lastlyHighlightedImage;

        private const float IMAGE_MOVE_AMOUNT = 4;
        private const float TRANSITION_DURATION = 0.1f;
        private const float HIGHLIGHT_DURATION = 0.32f;

        private void Start() =>  StartCoroutine(HighlightImageCoroutine());

        private IEnumerator HighlightImageCoroutine()
        {
            while (true)
            {
                Transform randomImage = GetRandomImage();
                Vector3 finalPosition = randomImage.position + (randomImage.up * IMAGE_MOVE_AMOUNT);

                randomImage.DOMove(finalPosition, TRANSITION_DURATION);
                if (_lastlyHighlightedImage != null) _lastlyHighlightedImage.DOMove(_initialPosition, TRANSITION_DURATION);

                _lastlyHighlightedImage = randomImage;
                _initialPosition = randomImage.position;

                yield return new WaitForSeconds(HIGHLIGHT_DURATION);
            }
        }

        private Transform GetRandomImage()
        {
            Transform randomImage;

            do
            {
                int randomImageIndex = Random.Range(0, images.Count);
                randomImage = images[randomImageIndex];

            } while (randomImage == _lastlyHighlightedImage);

            return randomImage;
        }
    }
}