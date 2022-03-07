using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Crush2048
{
    public class VolumeSettings : MonoBehaviour
    {
        [SerializeField] Settings settings;
        [SerializeField] Slider backgroundMusicSlider;
        [SerializeField] Slider soundEffectsSlider;

        private void Start() => LoadSettings();

        public void SetVolumeSettings()
        {
            settings.BackgroundMusicVolume = backgroundMusicSlider.value;
            settings.SoundEffectsVolume = soundEffectsSlider.value;
        }

        private void LoadSettings()
        {
            float backgroundMusicVolume = settings.BackgroundMusicVolume;
            float soundEffectsVolume = settings.SoundEffectsVolume;

            backgroundMusicSlider.value = backgroundMusicVolume;
            soundEffectsSlider.value = soundEffectsVolume;
        }
    }
}
