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

        private void LoadSettings()
        {
            backgroundMusicSlider.value = settings.BackgroundMusicVolume;
            soundEffectsSlider.value = settings.SoundEffectsVolume;
        }
    }
}
