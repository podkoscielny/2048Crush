using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Tags = MultipleTagSystem.TagSystem.Tags;

namespace Crush2048
{
    public class EffectHandler : MonoBehaviour
    {
        [SerializeField] ObjectPool objectPool;
        [SerializeField] EffectColor effectColor;
        [SerializeField] List<ParticleSystem> particleSystems;

        private const float DEACTIVATE_DELAY = 0.4f;

        private void OnEnable()
        {
            SetEffectColor();
            Invoke(nameof(DeactivateEffect), DEACTIVATE_DELAY);
        }

        private void DeactivateEffect() => objectPool.AddToPool(Tags.MatchEffect, gameObject);

        private void SetEffectColor()
        {
            //foreach (ParticleSystem particleSystem in particleSystems)
            //{
            //    var main = particleSystem.main;
            //    main.startColor = new ParticleSystem.MinMaxGradient(effectColor.Color);
            //}
        }
    }
}
