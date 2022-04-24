using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AoOkami.SaveSystem;

namespace Crush2048
{
    [CreateAssetMenu(fileName = "Settings", menuName = "ScriptableObjects/Settings")]
    public class Settings : ScriptableObject
    {
        public event Action OnSettingsChanged;

        [SerializeField] bool isVFXEnabled = true;
        [Range(0, 1)] [SerializeField] float backgroundMusicVolume = 1;
        [Range(0, 1)] [SerializeField] float soundEffectsVolume = 1;

        public bool IsVFXEnabled { get => isVFXEnabled; set { isVFXEnabled = value; SaveSettings(); } }
        public float BackgroundMusicVolume { get => backgroundMusicVolume; set { backgroundMusicVolume = value; SaveSettings(); } }
        public float SoundEffectsVolume { get => soundEffectsVolume; set { soundEffectsVolume = value; SaveSettings(); } }

        private const string SAVE_PATH = "settings";

        private void OnEnable() => LoadSettings();

#if UNITY_EDITOR
        private void OnValidate() => SaveSettings();
#endif

        private void SaveSettings()
        {
            PlayerPrefs.SetInt("HasSavedSettings", 1);
            PlayerPrefs.Save();

            SettingsData settingsData = new SettingsData(IsVFXEnabled, BackgroundMusicVolume, SoundEffectsVolume);

            SaveSystem.Save<SettingsData>(SAVE_PATH, settingsData);

            OnSettingsChanged?.Invoke();
        }

        private void LoadSettings()
        {
            SettingsData settingsData = PlayerPrefs.GetInt("HasSavedSettings") == 1 ? SaveSystem.Load<SettingsData>(SAVE_PATH) : GetDefaultSettings();

            isVFXEnabled = settingsData.IsVFXEnabled;
            backgroundMusicVolume = settingsData.BackgroundMusicVolume;
            soundEffectsVolume = settingsData.SoundEffectsVolume;
        }

        private SettingsData GetDefaultSettings() => new SettingsData(true, 1, 1);
    }
}
