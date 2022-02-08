using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Crush2048
{
    [System.Serializable]
    public struct CachedBaord
    {
        public int Score { get; private set; }
        public CachedTileType[,] CachedTileTypesAtCells { get; private set; }
        public int[,] CachedPointsAtCells { get; private set; }
        public bool IsGameOver { get; private set; }

        public CachedBaord(int score, CachedTileType[,] tileTypes, int[,] cachedPoints, bool isGameOver)
        {
            Score = score;
            CachedTileTypesAtCells = tileTypes;
            CachedPointsAtCells = cachedPoints;
            IsGameOver = isGameOver;
        }
    }
}
