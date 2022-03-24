using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Crush2048
{
    public class ResolutionPicker : MonoBehaviour
    {
        private const float MOBILE_RESOLUTION_FACTOR = 0.6f;

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSplashScreen)]
        private void Start() => SetPortraitResolution();

        private void SetPortraitResolution()
        {
#if UNITY_EDITOR
            return;
#endif

#if UNITY_STANDALONE
            int displayHeight = Display.main.systemHeight;

            int height = (int)(displayHeight * 0.7f);
            int width = (int)(height * 0.5625f);

            Screen.SetResolution(width, height, false);
#endif

#if UNITY_ANDROID
            Display display = Display.main;

            int height = (int)(display.systemHeight * MOBILE_RESOLUTION_FACTOR);
            int width = (int)(display.systemWidth * MOBILE_RESOLUTION_FACTOR);

            Screen.SetResolution(width, height, true);
#endif
        }
    }
}
