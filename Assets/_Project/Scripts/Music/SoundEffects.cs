using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Crush2048
{
    public class SoundEffects : MonoBehaviour
    {
        [SerializeField] AudioSource audioSource;

        private void OnEnable() => BaseBehaviour.OnSoundEffectPlay += PlaySoundEffect;

        private void OnDisable() => BaseBehaviour.OnSoundEffectPlay -= PlaySoundEffect;

        private void PlaySoundEffect(AudioClip audioClip)
        {
            audioSource.Stop();
            audioSource.clip = audioClip;
            audioSource.Play();
        }
    }
}
