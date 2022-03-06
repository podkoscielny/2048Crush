using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Crush2048
{
    public class EffectColor : MonoBehaviour
    {
        [SerializeField] TileGradients tileGradients;
        [SerializeField] Material effectMaterial;

        private void OnEnable() => SetEffectColor();

        private void SetEffectColor()
        {
            Color effectColor = tileGradients.RecentlyPickedBackgroundColor;

            effectMaterial.color = effectColor;
            effectMaterial.SetColor("_EmissionColor", effectColor);
        }
    }
}
