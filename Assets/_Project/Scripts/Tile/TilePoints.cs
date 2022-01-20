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

        private void OnEnable() => tileTypePicker.OnTileTypePicked += (tileType, isKeepingPoints) => InitializePoints(tileType, isKeepingPoints);

        private void OnDisable() => tileTypePicker.OnTileTypePicked -= InitializePoints;

        public void MultiplyPoints(int multiplier) => PointsWorth *= multiplier;

        public void SetPoints(int pointsAmount) => PointsWorth = pointsAmount;

        private void InitializePoints(TileType tileType, bool isKeepingPoints)
        {
            if (!isKeepingPoints) PointsWorth = tileType.pointsWorth;
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
