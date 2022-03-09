using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Crush2048
{
    public class MusicMute : MonoBehaviour
    {
        [SerializeField] AudioSource audioSource;

        public void MuteMusic() => audioSource.mute = !audioSource.mute;
    }
}
