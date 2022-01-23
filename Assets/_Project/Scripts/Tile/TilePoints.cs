using System;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Crush2048
{
    public class TilePoints : MonoBehaviour
    {
        public event Action OnPointsUpdated;

        [SerializeField] TileTypePicker tileTypePicker;
        [SerializeField] GridSystem gridSystem;

        public int PointsWorth
        {
            get => _pointsWorth;

            private set
            {
                _pointsWorth = value;
                OnPointsUpdated?.Invoke();
            }
        }

        private int _pointsWorth = 2;

        private void OnEnable()
        {
            Board.OnCacheTileValues += CachePointsWorthAtCell;
            Board.OnTilesReverse += SetCachedPointsWorth;
            tileTypePicker.OnTileTypePicked += InitializePoints;
        }

        private void OnDisable()
        {
            Board.OnCacheTileValues -= CachePointsWorthAtCell;
            Board.OnTilesReverse -= SetCachedPointsWorth;
            tileTypePicker.OnTileTypePicked -= InitializePoints;
        }

        public void MultiplyPoints(int multiplier) => PointsWorth *= multiplier;

        public void SetPoints(int pointsAmount) => PointsWorth = pointsAmount;

        private void CachePointsWorthAtCell()
        {
            Vector2Int tileCell = gridSystem.GetTileGridCell(gameObject);
            gridSystem.CachePointsWorthAtCell(tileCell, PointsWorth);
        }

        private void SetCachedPointsWorth()
        {
            Vector2Int tileCell = gridSystem.GetTileGridCell(gameObject);
            PointsWorth = gridSystem.PointsWorthAtCells[tileCell.x, tileCell.y];
        }

        private void InitializePoints(TileType tileType, bool isTileTypeCached)
        {
            if (!isTileTypeCached) PointsWorth = tileType.pointsWorth;
        }

#if UNITY_EDITOR
        public void ChangeTilePointsWorth(int pointsToSet) => PointsWorth = pointsToSet;

        public void RandomizeTilePoints()
        {
            int randomPoints;

            do
            {
                randomPoints = Random.Range(2, 512);

            } while (!IsPowerOfTwo(randomPoints));

            PointsWorth = randomPoints;
        }

        private bool IsPowerOfTwo(int x) => (x != 0) && ((x & (x - 1)) == 0);
#endif
    }
}
