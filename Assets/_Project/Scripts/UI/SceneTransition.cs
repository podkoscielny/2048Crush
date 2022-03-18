using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

namespace Crush2048
{
    public class SceneTransition : MonoBehaviour
    {
        private void Start() => StartCoroutine(MasterVolume.UnMute());
    }
}
