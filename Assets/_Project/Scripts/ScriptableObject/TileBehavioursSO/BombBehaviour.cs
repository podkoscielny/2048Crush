using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Tags = MultipleTagSystem.TagSystem.Tags;

namespace Crush2048
{
    [CreateAssetMenu(fileName = "BombBehaviour", menuName = "ScriptableObjects/TileBehaviours/BombBehaviour")]
    public class BombBehaviour : BaseBehaviour
    {
        public static event Action OnBombBehaviourInvoked;

        [SerializeField] int destroyInRowForEachSide;
        [SerializeField] int destroyInColumnForEachSide;

        public override void Invoke(SelectedTile firstSelectedTile, SelectedTile secondSelectedTile)
        {
            List<Vector2Int> nearbyTiles = GetNearbyTileCells(secondSelectedTile.TileCell);

            SpawnBombEffects(nearbyTiles);
            AddScoreFromNearbyTiles(nearbyTiles);
            MoveNearbyTilesToPool(nearbyTiles);
            OnBombBehaviourInvoked?.Invoke();
        }

        private List<Vector2Int> GetNearbyTileCells(Vector2Int tileCell)
        {
            int rows = gridSystem.GridSize.Rows;
            int columns = gridSystem.GridSize.Columns;

            List<Vector2Int> nearbyTiles = new List<Vector2Int>();

            nearbyTiles.Add(tileCell);

            for (int i = 0; i < destroyInRowForEachSide; i++)
            {
                Vector2Int rightTileCellToAdd = new Vector2Int(tileCell.x, tileCell.y + i + 1);
                Vector2Int leftTileCellToAdd = new Vector2Int(tileCell.x, tileCell.y - i - 1);

                if (rightTileCellToAdd.y < columns) nearbyTiles.Add(rightTileCellToAdd);
                if (leftTileCellToAdd.y >= 0) nearbyTiles.Add(leftTileCellToAdd);
            }

            for (int i = 0; i < destroyInColumnForEachSide; i++)
            {
                Vector2Int upTileCellToAdd = new Vector2Int(tileCell.x - i - 1, tileCell.y);
                Vector2Int downTileCellToAdd = new Vector2Int(tileCell.x + i + 1, tileCell.y);

                if (upTileCellToAdd.x >= 0) nearbyTiles.Add(upTileCellToAdd);
                if (downTileCellToAdd.x < rows) nearbyTiles.Add(downTileCellToAdd);
            }

            return nearbyTiles;
        }

        private void AddScoreFromNearbyTiles(List<Vector2Int> nearbyTiles)
        {
            TileType[,] cachedTileValues = gridSystem.CachedTilesAtCells;
            int[,] cachedPointsWorth = gridSystem.CachedPointsWorthAtCells;

            int scoreToAdd = 0;

            foreach (Vector2Int cell in nearbyTiles)
            {
                if (!cachedTileValues[cell.x, cell.y].isSpecial) scoreToAdd += cachedPointsWorth[cell.x, cell.y];
            }

            UpdateScore(scoreToAdd);
        }

        private void MoveNearbyTilesToPool(List<Vector2Int> nearbyTiles)
        {
            foreach (Vector2Int cell in nearbyTiles)
            {
                GameObject tile = gridSystem.TilesAtGridCells[cell.x, cell.y];

                MoveTileToPool(cell, tile);
            }
        }

        private void SpawnBombEffects(List<Vector2Int> nearbyTiles)
        {
            List<Vector3> spawnPositions = GetSpawnPositions(nearbyTiles);

            foreach (Vector3 position in spawnPositions)
            {
                SpawnParticleEffect(position, Tags.BombEffect);
            }
        }

        private List<Vector3> GetSpawnPositions(List<Vector2Int> nearbyTiles)
        {
            List<Vector3> spawnPositions = new List<Vector3>();

            foreach (Vector2Int cell in nearbyTiles)
            {
                Vector3 spawnPosition = gridSystem.TilesAtGridCells[cell.x, cell.y].transform.position;

                spawnPositions.Add(spawnPosition);
            }

            return spawnPositions;
        }
    }
}
