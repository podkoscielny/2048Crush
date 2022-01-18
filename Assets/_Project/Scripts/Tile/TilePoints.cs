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
            Board.OnAssignPointsWorthToCells += AssignPointsWorthToCell;
            Board.OnTilesReverse += ReversePoints;
            tileTypePicker.OnTileTypePicked += InitializePoints;
        }

        private void OnDisable()
        {
            Board.OnAssignPointsWorthToCells -= AssignPointsWorthToCell;
            Board.OnTilesReverse -= ReversePoints;
            tileTypePicker.OnTileTypePicked -= InitializePoints;
        }

        public void MultiplyPoints(int multiplier) => PointsWorth *= multiplier;

        public void SetPoints(int pointsAmount) => PointsWorth = pointsAmount;

        private void InitializePoints(TileType tileType) => PointsWorth = tileType.pointsWorth;

        private void ReversePoints()
        {
            Vector2Int tileCell = gridSystem.GetTileGridCell(gameObject);
            PointsWorth = gridSystem.CachedPointsWorthAtCells[tileCell.x, tileCell.y];
        }

        private void AssignPointsWorthToCell()
        {
            Vector2Int tileCell = gridSystem.GetTileGridCell(gameObject);
            gridSystem.AssignPointsWorthToCell(PointsWorth, tileCell);
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
