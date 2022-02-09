using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Tags = MultipleTagSystem.TagSystem.Tags;

namespace Crush2048
{
    public class EffectHandler : MonoBehaviour
    {
        [SerializeField] ObjectPool objectPool;

        private const float DEACTIVATE_DELAY = 0.4f;

        private void OnEnable() => Invoke(nameof(DeactivateEffect), DEACTIVATE_DELAY);

        private void DeactivateEffect() => objectPool.AddToPool(Tags.MatchEffect, gameObject);
    }
}
