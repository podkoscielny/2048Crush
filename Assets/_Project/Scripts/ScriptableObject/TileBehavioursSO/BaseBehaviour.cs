using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Tags = MultipleTagSystem.TagSystem.Tags;
using DG.Tweening;

namespace Crush2048
{
    public abstract class BaseBehaviour : ScriptableObject
    {
        public static event Action<AudioClip> OnSoundEffectPlay;

        [SerializeField] protected GridSystem gridSystem;
        [SerializeField] protected ObjectPool objectPool;
        [SerializeField] protected Settings settings;
        [SerializeField] protected Score score;

        protected Quaternion initialRotation = new Quaternion(0, 0, 0, 0);
        private Vector3 _enlargedTileScale = new Vector3(0.4f, 0.4f, 0.4f);

        public abstract void Invoke(SelectedTile firstSelectedTile, SelectedTile secondSelectedTile);

        protected void MoveTileToPool(Vector2Int cell, GameObject tile)
        {
            gridSystem.DeAssignTileFromCell(cell);
            objectPool.AddToPool(tile);
        }

        protected void SpawnParticleEffect(Vector3 spawnPosition, Tags specialEffectTag)
        {
            if (!settings.IsVFXEnabled) return;

            GameObject particleEffect = objectPool.GetFromPool(specialEffectTag);
            particleEffect.transform.position = spawnPosition;
        }

        protected void PlaySoundEffect(AudioClip audioClip) => OnSoundEffectPlay?.Invoke(audioClip);

        protected void UpdateScore(int pointsToAdd) => score.AddPoints(pointsToAdd);

        protected void MultiplyTilePoints(TilePoints tilePointsToBeUpdated, int multiplier) => tilePointsToBeUpdated.MultiplyPoints(multiplier);

        protected void DoPunchScale(Transform tileTransform) => tileTransform.DOPunchScale(_enlargedTileScale, 0.3f, 1);
    }
}
