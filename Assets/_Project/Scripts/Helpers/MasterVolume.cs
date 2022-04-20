using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

namespace Crush2048
{
    public static class MasterVolume
    {
        private static Tween _muteTween;
        private static bool _isMuting = false;
        private static readonly float _changeVolumeDuration = 0.7f;

        public static void MuteTest(Action callback = null)
        {
            _isMuting = true;

            _muteTween = DOTween.To(() => AudioListener.volume, x => AudioListener.volume = x, 0, _changeVolumeDuration)
                .OnComplete(() => MuteCallback(callback));
        }

        public static void UnMuteTest()
        {
            if (_isMuting)
            {
                _muteTween.Kill();
                _isMuting = false;
            }

            DOTween.To(() => AudioListener.volume, x => AudioListener.volume = x, 1, _changeVolumeDuration);
        }

        private static void MuteCallback(Action AdditionalCallback = null)
        {
            _isMuting = false;
            AdditionalCallback.Invoke();
        }
    }
}
