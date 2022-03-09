using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Crush2048
{
    public class BackgroundMusic : MonoBehaviour
    {
        [SerializeField] AudioSource audioSource;
        [SerializeField] Settings settings;

        private float _baseVolume = 1;

        private void OnEnable() => settings.OnSettingsChanged += SetVolume;

        private void OnDisable() => settings.OnSettingsChanged -= SetVolume;

        private void Start()
        {
            SetBaseVolume();
            SetVolume();
        }

        private void SetBaseVolume() => _baseVolume = audioSource.volume;

        private void SetVolume() => audioSource.volume = _baseVolume * settings.BackgroundMusicVolume;
    }
}
