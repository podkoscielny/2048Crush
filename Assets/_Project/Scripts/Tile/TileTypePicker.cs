using System;
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
            Board.OnTilesReverse += GetCachedTileType;

            TileType = GetRandomTileType();
            OnTileTypePicked?.Invoke(TileType);
        }

        private void OnDisable()
        {
            Board.OnCacheTileValues -= CacheTileType;
            Board.OnTilesReverse -= GetCachedTileType;
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

        private TileType GetRandomTileType()
        {
            TileProbabilityPair[] tileTypePairs = gridSystem.GridSize.TileTypes;
            float probabilitySum = gridSystem.GridSize.ProbabilitySum;
            float randomProbability = Random.Range(0, probabilitySum);
            float subtractFromSum = 0;

            for (int i = 0; i < tileTypePairs.Length; i++)
            {
                if (randomProbability - subtractFromSum <= tileTypePairs[i].spawnProbability) return tileTypePairs[i].tileType;

                subtractFromSum -= tileTypePairs[i].spawnProbability;
            }

            return tileTypePairs[tileTypePairs.Length - 1].tileType;
        }
    }
}
