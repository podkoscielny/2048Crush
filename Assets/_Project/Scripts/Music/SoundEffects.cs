using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Crush2048
{
    public class SoundEffects : SoundHandler
    {
        [SerializeField] AudioClip gameOverSound;

        protected override void OnEnable()
        {
            base.OnEnable();
            BaseBehaviour.OnSoundEffectPlay += PlaySoundEffect;
            TileMatchSequence.OnGameOver += PlayGameOverSound;
        }

        protected override void OnDisable()
        {
            base.OnDisable();
            BaseBehaviour.OnSoundEffectPlay -= PlaySoundEffect;
            TileMatchSequence.OnGameOver -= PlayGameOverSound;
        }

        protected override void SetVolume() => audioSource.volume = _baseVolume * settings.SoundEffectsVolume;

        private void PlaySoundEffect(AudioClip audioClip)
        {
            audioSource.Stop();
            audioSource.clip = audioClip;
            audioSource.Play();
        }

        private void PlayGameOverSound() => PlaySoundEffect(gameOverSound);
    }
}
