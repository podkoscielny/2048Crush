using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

namespace Crush2048
{
    public class SceneTransition : MonoBehaviour
    {
        private float _volumeRef;

        private void Start() => StartCoroutine(UnMuteMasterVolume());

        private IEnumerator UnMuteMasterVolume()
        {
            while (AudioListener.volume < 1f)
            {
                AudioListener.volume = Mathf.SmoothDamp(AudioListener.volume, 1, ref _volumeRef, 0.2f);
                yield return null;
            }
        }
    }
}
