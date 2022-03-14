using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Crush2048
{
    public static class MasterVolume
    {
        public static void MuteMasterVolume()
        {

        }

        public static void UnMuteMasterVolume()
        {
            float masterVolume = AudioListener.volume;

        }
    }
}
