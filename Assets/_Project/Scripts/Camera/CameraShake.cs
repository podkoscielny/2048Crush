using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Crush2048
{
    public class CameraShake : MonoBehaviour
    {
        [SerializeField] Settings settings;

        private const float DURATION = 0.5f;
        private const float MAGNITUDE = 0.05f;
        private const float SHAKE_RANGE = 2f;

        private void OnEnable() => BombBehaviour.OnBombBehaviourInvoked += ShakeCamera;

        private void OnDisable() => BombBehaviour.OnBombBehaviourInvoked -= ShakeCamera;

        public void ShakeCamera()
        {
            if (!settings.IsVFXEnabled) return;

            StopAllCoroutines();
            StartCoroutine(ShakeCameraCoroutine());
        }

        private IEnumerator ShakeCameraCoroutine()
        {
            Vector3 originalCameraPosition = transform.position;

            float timeElapsed = 0f;

            while (timeElapsed < DURATION)
            {
                float timeFactor = DURATION - timeElapsed;

                float xPos = Random.Range(-SHAKE_RANGE, SHAKE_RANGE) * MAGNITUDE * timeFactor;
                float yPos = Random.Range(-SHAKE_RANGE, SHAKE_RANGE) * MAGNITUDE * timeFactor;

                Vector3 cameraShakePosition = new Vector3(xPos, yPos, 0);

                transform.localPosition = originalCameraPosition + cameraShakePosition;

                timeElapsed += Time.deltaTime;

                yield return null;
            }

            transform.position = originalCameraPosition;
        }
    }
}
