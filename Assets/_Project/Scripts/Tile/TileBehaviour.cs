using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using Tags = MultipleTagSystem.TagSystem.Tags;

namespace Crush2048
{
    public class TileBehaviour : MonoBehaviour
    {
        [SerializeField] Score score;
        [SerializeField] ObjectPool objectPool;
        [SerializeField] GridSystem gridSystem;
        [SerializeField] TilePoints tilePoints;
        [SerializeField] TileTypePicker tileTypePicker;

        public bool IsSpecial { get; private set; } = false;
        public BehaviourDelegate Behaviour { get; private set; }

        private Vector3 _enlargedTileScale = new Vector3(0.4f, 0.4f, 0.4f);

        private void OnEnable()
        {
            tileTypePicker.OnTileTypePicked += CacheTileBehaviour;
            tileTypePicker.OnGetCachedTileType += CacheTileBehaviour;
        }

        private void OnDisable()
        {
            tileTypePicker.OnTileTypePicked -= CacheTileBehaviour;
            tileTypePicker.OnGetCachedTileType += CacheTileBehaviour;
        }

        private void DefaultBehaviour(SelectedTile firstSelectedTile, SelectedTile secondSelectedTile)
        {
            Vector3 spawnPosition = secondSelectedTile.TileObject.transform.position;
            spawnPosition.z -= 0.1f;

            MoveTileToPool(firstSelectedTile.TileCell, firstSelectedTile.TileObject);
            UpdateScore(tilePoints, 2);
            SpawnParticleEffect(spawnPosition);
            DoPunchScale(secondSelectedTile.TileObject.transform);
        }

        private void DoPunchScale(Transform tileTransform) => tileTransform.DOPunchScale(_enlargedTileScale, 0.3f, 1);

        private void UpdateScore(TilePoints tilePointsToBeUpdated, int multiplier)
        {
            tilePointsToBeUpdated.MultiplyPoints(multiplier);
            score.AddPoints(tilePointsToBeUpdated.PointsWorth);
        }

        private void MoveTileToPool(Vector2Int cell, GameObject tile)
        {
            gridSystem.DeAssignTileFromCell(cell);
            objectPool.AddToPool(Tags.Tile, tile);
        }

        private void SpawnParticleEffect(Vector3 spawnPosition)
        {
            GameObject particleEffect = objectPool.GetFromPool(Tags.MatchEffect);
            particleEffect.transform.position = spawnPosition;
        }

        private void CacheTileBehaviour(TileType tileType)
        {
            IsSpecial = tileType.isSpecial;

            if (IsSpecial)
                Behaviour = tileType.specialBehaviour.Invoke;
            else
                Behaviour = DefaultBehaviour;
        }
    }

    public delegate void BehaviourDelegate(SelectedTile firstSelectedTile, SelectedTile secondSelectedTile);
}
