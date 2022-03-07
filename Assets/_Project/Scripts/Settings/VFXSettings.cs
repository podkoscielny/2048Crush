using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Crush2048
{
    public class VFXSettings : MonoBehaviour
    {
        [SerializeField] Settings settings;
        [SerializeField] Toggle toggle;

        private void Start() => LoadSettings();

        public void SetVFXSettings() => settings.IsVFXEnabled = toggle.isOn;

        private void LoadSettings() => toggle.isOn = settings.IsVFXEnabled;

    }
}
