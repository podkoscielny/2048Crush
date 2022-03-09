using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Crush2048
{
    public class SoundEffects : SoundHandler
    {
        protected override void OnEnable()
        {
            base.OnEnable();
            BaseBehaviour.OnSoundEffectPlay += PlaySoundEffect;
        }

        protected override void OnDisable()
        {
            base.OnDisable();
            BaseBehaviour.OnSoundEffectPlay -= PlaySoundEffect;
        }

        protected override void SetVolume() => audioSource.volume = _baseVolume * settings.SoundEffectsVolume;

        private void PlaySoundEffect(AudioClip audioClip)
        {
            audioSource.Stop();
            audioSource.clip = audioClip;
            audioSource.Play();
        }
    }
}
