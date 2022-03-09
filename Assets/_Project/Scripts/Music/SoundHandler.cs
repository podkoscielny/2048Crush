using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Crush2048
{
    public abstract class SoundHandler : MonoBehaviour
    {
        [SerializeField] protected AudioSource audioSource;
        [SerializeField] protected Settings settings;

        protected float _baseVolume = 1f;

        protected virtual void OnEnable() => settings.OnSettingsChanged += SetVolume;

        protected virtual void OnDisable() => settings.OnSettingsChanged -= SetVolume;

        protected virtual void Start()
        {
            SetBaseVolume();
            SetVolume();
        }

        protected abstract void SetVolume();

        private void SetBaseVolume() => _baseVolume = audioSource.volume;
    }
}
