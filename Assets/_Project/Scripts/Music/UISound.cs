using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Crush2048
{
    public class UISound : SoundHandler
    {
        [SerializeField] AudioClip buttonUpSound;
        [SerializeField] AudioClip buttonDownSound;

        protected override void OnEnable()
        {
            base.OnEnable();
            ButtonSound.OnButtonUp += PlayOnButtonUp;
            ButtonSound.OnButtonClick += PlayOnButtonUp;
            ButtonSound.OnButtonDown += PlayOnButtonDown;
        }

        protected override void OnDisable()
        {
            base.OnDisable();
            ButtonSound.OnButtonUp -= PlayOnButtonUp;
            ButtonSound.OnButtonClick -= PlayOnButtonUp;
            ButtonSound.OnButtonDown -= PlayOnButtonDown;
        }

        public void PlaySoundEffect(AudioClip audioClip)
        {
            audioSource.Stop();
            audioSource.clip = audioClip;
            audioSource.Play();
        }

        private void PlayOnButtonUp() => PlaySoundEffect(buttonUpSound);

        private void PlayOnButtonDown() => PlaySoundEffect(buttonDownSound);

        protected override void SetVolume() => audioSource.volume = _baseVolume * settings.SoundEffectsVolume;
    }
}
