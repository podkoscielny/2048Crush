using System;

namespace Crush2048
{
    [Serializable]
    public struct SettingsData
    {
        public bool IsVFXEnabled { get; private set; }
        public float BackgroundMusicVolume { get; private set; }
        public float SoundEffectsVolume { get; private set; }

        public SettingsData(bool isVFXEnabled, float backgroundMusicVolume, float soundEffectsVolume)
        {
            IsVFXEnabled = isVFXEnabled;
            BackgroundMusicVolume = backgroundMusicVolume;
            SoundEffectsVolume = soundEffectsVolume;
        }
    }
}
