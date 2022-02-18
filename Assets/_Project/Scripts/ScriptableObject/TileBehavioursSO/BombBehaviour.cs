using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Crush2048
{
    [CreateAssetMenu(fileName = "BombBehaviour", menuName = "ScriptableObjects/TileBehaviours/BombBehaviour")]
    public class BombBehaviour : BaseBehaviour
    {
        [SerializeField] int destroyInRowForEachSide;
        [SerializeField] int destroyInColumnForEachSide;

        public override void Invoke(SelectedTile firstSelectedTile, SelectedTile secondSelectedTile)
        {
            List<Vector2Int> nearbyTiles = GetNearbyTileCells(secondSelectedTile.TileCell);

            AddScoreFromNearbyTiles(nearbyTiles);
            MoveNearbyTilesToPool(nearbyTiles);
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

                if (upTileCellToAdd.y >= 0) nearbyTiles.Add(upTileCellToAdd);
                if (downTileCellToAdd.y < rows) nearbyTiles.Add(downTileCellToAdd);
            }

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

            UpdateScore(scoreToAdd);
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
    }
}