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
        public Behaviours BehaviourEnum { get; private set; } = Behaviours.Default;

        private Quaternion _initialRotation = new Quaternion(0, 0, 0, 0);
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

        private void MatchAnyTile(SelectedTile firstSelectedTile, SelectedTile secondSelectedTile)
        {
            if (secondSelectedTile.TileBehaviour.IsSpecial)
            {
                Transform firstSelectedTransform = firstSelectedTile.TileObject.transform;
                firstSelectedTransform.rotation = _initialRotation;

                gridSystem.AssignTileToCell(firstSelectedTile.TileObject, secondSelectedTile.TileCell);
                MoveTileToPool(firstSelectedTile.TileCell, gameObject);

                DoPunchScale(firstSelectedTransform);
            }
            else
            {
                MoveTileToPool(firstSelectedTile.TileCell, firstSelectedTile.TileObject);
                DoPunchScale(secondSelectedTile.TileObject.transform);
            }
        }

        private void MultiplyTilePoints(SelectedTile firstSelectedTile, SelectedTile secondSelectedTile)
        {
            if (secondSelectedTile.TileBehaviour.IsSpecial)
            {
                Transform firstSelectedTransform = firstSelectedTile.TileObject.transform;
                firstSelectedTransform.rotation = _initialRotation;

                gridSystem.AssignTileToCell(firstSelectedTile.TileObject, secondSelectedTile.TileCell);
                MoveTileToPool(firstSelectedTile.TileCell, gameObject);

                UpdateScore(firstSelectedTile.TilePoints, 4);
                DoPunchScale(firstSelectedTransform);
            }
            else
            {
                MoveTileToPool(firstSelectedTile.TileCell, firstSelectedTile.TileObject);

                UpdateScore(secondSelectedTile.TilePoints, 4);
                DoPunchScale(secondSelectedTile.TileObject.transform);
            }
        }

        private void BombNearbyTiles(SelectedTile firstSelectedTile, SelectedTile secondSelectedTile)
        {
            List <Vector2Int> nearbyTiles = GetNearbyTileCells(secondSelectedTile.TileCell);

            AddScoreFromNearbyTiles(nearbyTiles);
            MoveNearbyTilesToPool(nearbyTiles);
        }

        private void DoPunchScale(Transform tileTransform) => tileTransform.DOPunchScale(_enlargedTileScale, 0.3f, 1);

        private void UpdateScore(TilePoints tilePointsToBeUpdated, int multiplier)
        {
            tilePointsToBeUpdated.MultiplyPoints(multiplier);
            score.AddPoints(tilePointsToBeUpdated.PointsWorth);
        }

        private List<Vector2Int> GetNearbyTileCells(Vector2Int tileCell)
        {
            List<Vector2Int> nearbyTiles = new List<Vector2Int>() 
            { 
                tileCell, 
                new Vector2Int(tileCell.x + 1, tileCell.y) ,
                new Vector2Int(tileCell.x - 1, tileCell.y) ,
                new Vector2Int(tileCell.x, tileCell.y + 1) ,
                new Vector2Int(tileCell.x, tileCell.y - 1) ,
            };

            return nearbyTiles;
        }

        private void AddScoreFromNearbyTiles(List<Vector2Int> nearbyTiles)
        {
            int rows = gridSystem.GridSize.Rows;
            int columns = gridSystem.GridSize.Columns;

            TileType[,] cachedTileValues = gridSystem.CachedTilesAtCells;
            int[,] cachedPointsWorth = gridSystem.CachedPointsWorthAtCells;

            int scoreToAdd = 0;

            foreach (Vector2Int cell in nearbyTiles)
            {
                if (cell.x < 0 || cell.x >= rows || cell.y < 0 || cell.y >= columns) continue;

                if (!cachedTileValues[cell.x, cell.y].isSpecial) scoreToAdd += cachedPointsWorth[cell.x, cell.y];
            }

            score.AddPoints(scoreToAdd);
        }

        private void MoveNearbyTilesToPool(List<Vector2Int> nearbyTiles)
        {
            int rows = gridSystem.GridSize.Rows;
            int columns = gridSystem.GridSize.Columns;

            foreach (Vector2Int cell in nearbyTiles)
            {
                if (cell.x < 0 || cell.x >= rows || cell.y < 0 || cell.y >= columns) continue;

                GameObject tile = gridSystem.TilesAtGridCells[cell.x, cell.y];

                MoveTileToPool(cell, tile);
            }
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

            if (!tileType.isSpecial)
            {
                Behaviour = DefaultBehaviour;
                return;
            }

            switch (tileType.tileBehaviour)
            {
                case Behaviours.Default:
                    Behaviour = DefaultBehaviour;
                    break;

                case Behaviours.MultiplyAnyTile:
                    Behaviour = MultiplyTilePoints;
                    break;

                case Behaviours.MatchAnyTile:
                    Behaviour = MatchAnyTile;
                    break;

                case Behaviours.BombNearbyTiles:
                    Behaviour = BombNearbyTiles;
                    break;

                default:
                    break;
            }
        }
    }

    public enum Behaviours
    {
        Default,
        MultiplyAnyTile,
        MatchAnyTile,
        BombNearbyTiles
    }

    public delegate void BehaviourDelegate(SelectedTile firstSelectedTile, SelectedTile secondSelectedTile);
}
