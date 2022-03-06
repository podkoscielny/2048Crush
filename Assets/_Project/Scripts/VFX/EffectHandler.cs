using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Tags = MultipleTagSystem.TagSystem.Tags;

namespace Crush2048
{
    public class EffectHandler : MonoBehaviour
    {
        [SerializeField] ObjectPool objectPool;
        [SerializeField] TileGradients tileGradients;
        [SerializeField] Material effectMaterial;

        private const float DEACTIVATE_DELAY = 0.4f;

        private void OnEnable()
        {
            SetEffectColor();
            Invoke(nameof(DeactivateEffect), DEACTIVATE_DELAY);
        }

        private void DeactivateEffect() => objectPool.AddToPool(gameObject);

        private void SetEffectColor()
        {
            Color effectColor = tileGradients.RecentlyPickedBackgroundColor;

            effectMaterial.color = effectColor;
            effectMaterial.SetColor("_EmissionColor", effectColor);
        }
    }
}
