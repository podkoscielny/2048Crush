using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Crush2048
{
#if UNITY_STANDALONE
    public class ResolutionPicker : MonoBehaviour
    {
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSplashScreen)]
        private void Start() => SetPortraitResolution();

        private void SetPortraitResolution()
        {
            int displayHeight = Display.main.systemHeight;

            int height = displayHeight - (int)(displayHeight * 0.1f);
            int width = (int)(height * 0.5625f);

            Screen.SetResolution(width, height, false);
        }
    }
#endif
}
