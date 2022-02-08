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
        [SerializeField] List<TileType> tileTypes;

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
            CachedTileType cachedTileType = cachedBoard.CachedTileTypesAtCells[tileCell.x, tileCell.y];

            TileType convertedTileType = ConvertSerializableTileTypeToNormal(cachedTileType);

            if (convertedTileType != null) TileType = convertedTileType;

            OnGetCachedTileType?.Invoke(TileType);
        }

        private TileType ConvertSerializableTileTypeToNormal(CachedTileType cachedTileType)
        {
            foreach (TileType tileType in tileTypes)
            {
                if (tileType.isSpecial == cachedTileType.IsSpecial && tileType.pointsWorth == cachedTileType.PointsWorth && tileType.tileBehaviour == cachedTileType.TileBehaviour)
                    return tileType;
            }

            return null;
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
