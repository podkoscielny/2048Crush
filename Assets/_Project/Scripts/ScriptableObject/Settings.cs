using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AoOkami.SaveSystem;

namespace Crush2048
{
    [CreateAssetMenu(fileName = "Settings", menuName = "ScriptableObjects/Settings")]
    public class Settings : ScriptableObject
    {
        [SerializeField] bool isVFXEnabled;
        [Range(0, 1)] [SerializeField] float backgroundMusicVolume;
        [Range(0, 1)] [SerializeField] float soundEffectsVolume;

        public bool IsVFXEnabled { get => isVFXEnabled; set { isVFXEnabled = value; SaveSettings(); } }
        public float BackgroundMusicVolume { get => backgroundMusicVolume; set { backgroundMusicVolume = value; SaveSettings(); } }
        public float SoundEffectsVolume { get => soundEffectsVolume; set { soundEffectsVolume = value; SaveSettings(); } }

        private const string SAVE_PATH = "settings";

        private void OnEnable() => LoadSettings();

        private void OnValidate() => SaveSettings();

        private void SaveSettings()
        {
            SettingsData settingsData = new SettingsData(IsVFXEnabled, BackgroundMusicVolume, SoundEffectsVolume);

            SaveSystem.Save<SettingsData>(SAVE_PATH, settingsData);
        }

        private void LoadSettings()
        {
            SettingsData settingsData = SaveSystem.Load<SettingsData>(SAVE_PATH);

            IsVFXEnabled = settingsData.IsVFXEnabled;
            BackgroundMusicVolume = settingsData.BackgroundMusicVolume;
            SoundEffectsVolume = settingsData.SoundEffectsVolume;
        }
    }
}
