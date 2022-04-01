using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Crush2048
{
    public class TargetFPSSetter : MonoBehaviour
    {
        private int _targetFramerate = -1;
        private int _vSyncCount = 1;

        private void Start() => SetTargetFPS();

        private void SetTargetFPS()
        {
#if UNITY_ANDROID
            _targetFramerate = -1;
            _vSyncCount = 1;
#endif

#if UNITY_STANDALONE
            _targetFramerate = -1;
            _vSyncCount = 1;
#endif
            Application.targetFrameRate = _targetFramerate;
            QualitySettings.vSyncCount = _vSyncCount;
        }
    }
}
