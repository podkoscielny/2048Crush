using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Crush2048
{
    public class TileTypePicker : MonoBehaviour
    {
        public event Action<TileType> OnTileTypePicked;
        public event Action<TileType> OnGetCachedTileType;

        [SerializeField] GridSystem gridSystem;

        public TileType TileType { get; private set; }

        private void OnEnable()
        {
            Board.OnCacheTileValues += CacheTileType;
            MoveReverse.OnTilesReverse += GetCachedTileType;
            Board.OnAssignTileValues += AssignTileTypeToCell;
            Board.OnCachedValuesLoaded += LoadCachedTileType;

            TileType = GetRandomTileType();
            OnTileTypePicked?.Invoke(TileType);
        }

        private void OnDisable()
        {
            Board.OnCacheTileValues -= CacheTileType;
            MoveReverse.OnTilesReverse -= GetCachedTileType;
            Board.OnAssignTileValues -= AssignTileTypeToCell;
            Board.OnCachedValuesLoaded -= LoadCachedTileType;
        }

        private void AssignTileTypeToCell()
        {
            Vector2Int tileCell = gridSystem.GetTileGridCell(gameObject);
            gridSystem.AssignTileTypeAtCell(tileCell, TileType);
        }

        private void CacheTileType()
        {
            Vector2Int tileCell = gridSystem.GetTileGridCell(gameObject);
            gridSystem.CacheTileTypeAtCell(tileCell, TileType);
        }

        private void GetCachedTileType()
        {
            Vector2Int tileCell = gridSystem.GetTileGridCell(gameObject);
            TileType tileType = gridSystem.CachedTilesAtCells[tileCell.x, tileCell.y];
            TileType = tileType;

            OnGetCachedTileType?.Invoke(tileType);
        }

        private void LoadCachedTileType(CachedBaord cachedBoard)
        {
            Vector2Int tileCell = gridSystem.GetTileGridCell(gameObject);
            CachedTileType cachedTileType = cachedBoard.TileTypesAtCells[tileCell.x, tileCell.y];
            
            TileType convertedTileType = TileTypeConverter.SerializableToNormal(cachedTileType, gridSystem.GridSize.GetTileTypeVariants());

            if (convertedTileType != null) TileType = convertedTileType;

            OnGetCachedTileType?.Invoke(TileType);
        }

        private TileType GetRandomTileType()
        {
            TileProbabilityPair[] tileTypePairs = gridSystem.GridSize.TileTypes;
            float probabilitySum = gridSystem.GridSize.ProbabilitySum;
            float randomProbability = Random.Range(0, probabilitySum);
            float subtractFromSum = 0;

            for (int i = 0; i < tileTypePairs.Length; i++)
            {
                if (randomProbability - subtractFromSum <= tileTypePairs[i].spawnProbability) return tileTypePairs[i].tileType;

                subtractFromSum += tileTypePairs[i].spawnProbability;
            }

            return tileTypePairs[tileTypePairs.Length - 1].tileType;
        }
    }
}
