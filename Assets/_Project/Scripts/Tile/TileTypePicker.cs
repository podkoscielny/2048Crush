using System;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Crush2048
{
    public class TileTypePicker : MonoBehaviour
    {
        public event Action<TileType> OnTileTypePicked;

        [SerializeField] GridSystem gridSystem;

        private void OnEnable()
        {
            TileType tileType = GetRandomTileType();
            OnTileTypePicked?.Invoke(tileType);
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
