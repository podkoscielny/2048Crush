using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Crush2048
{
    public static class MasterVolume
    {
        private static float _volumeRef;
        private static readonly float _changeVolumeDuration = 0.3f;

        private static bool _isMuting = false;

        public static bool IsMuted() => AudioListener.volume <= 0.01f;

        public static IEnumerator Mute()
        {
            _isMuting = true;

            while (AudioListener.volume > 0.01f)
            {
                AudioListener.volume = Mathf.SmoothDamp(AudioListener.volume, 0, ref _volumeRef, _changeVolumeDuration);
                yield return null;
            }

            _isMuting = false;
        }

        public static IEnumerator UnMute()
        {
            while (AudioListener.volume < 0.999f)
            {
                if (_isMuting) yield break;

                AudioListener.volume = Mathf.SmoothDamp(AudioListener.volume, 1, ref _volumeRef, _changeVolumeDuration);
                yield return null;
            }
        }
    }
}
