using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Crush2048
{
    public class UISound : MonoBehaviour
    {
        [SerializeField] AudioSource audioSource;
        [SerializeField] Settings settings;
        [SerializeField] AudioClip buttonUpSound;
        [SerializeField] AudioClip buttonDownSound;

        private float _baseVolume = 1;

        private void OnEnable()
        {
            ButtonSound.OnButtonUp += PlayOnButtonUp;
            ButtonSound.OnButtonClick += PlayOnButtonUp;
            ButtonSound.OnButtonDown += PlayOnButtonDown;
            settings.OnSettingsChanged += SetVolume;
        }

        private void OnDisable()
        {
            ButtonSound.OnButtonUp -= PlayOnButtonUp;
            ButtonSound.OnButtonClick -= PlayOnButtonUp;
            ButtonSound.OnButtonDown -= PlayOnButtonDown;
            settings.OnSettingsChanged -= SetVolume;
        }

        private void Start()
        {
            SetBaseVolume();
            SetVolume();
        }

        public void PlaySoundEffect(AudioClip audioClip)
        {
            audioSource.Stop();
            audioSource.clip = audioClip;
            audioSource.Play();
        }

        private void PlayOnButtonUp() => PlaySoundEffect(buttonUpSound);
        private void PlayOnButtonDown() => PlaySoundEffect(buttonDownSound);

        private void SetBaseVolume() => _baseVolume = audioSource.volume;

        private void SetVolume() => audioSource.volume = _baseVolume * settings.SoundEffectsVolume;
    }
}
