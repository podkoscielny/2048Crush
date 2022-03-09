using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Crush2048
{
    public class BackgroundMusic : SoundHandler
    {
        protected override void SetVolume() => audioSource.volume = _baseVolume * settings.BackgroundMusicVolume;
    }
}
