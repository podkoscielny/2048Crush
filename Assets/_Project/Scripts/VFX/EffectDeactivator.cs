using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Crush2048
{
    public class EffectDeactivator : MonoBehaviour
    {
        [SerializeField] ObjectPool objectPool;
        [SerializeField] float deactivateDelay = 0.4f;

        private const float DEACTIVATE_DELAY = 0.4f;

        private void OnEnable() => Invoke(nameof(DeactivateEffect), deactivateDelay);

        private void OnDisable() => CancelInvoke(nameof(DeactivateEffect));

        private void DeactivateEffect() => objectPool.AddToPool(gameObject);
    }
}
