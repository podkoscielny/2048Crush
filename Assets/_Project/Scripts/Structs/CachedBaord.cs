using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Crush2048
{
    [System.Serializable]
    public struct CachedBaord
    {
        public int Score { get; private set; }
        public int CachedScore { get; private set; }
        public CachedTileType[,] TileTypesAtCells { get; private set; }
        public CachedTileType[,] CachedTileTypesAtCells { get; private set; }
        public int[,] PointsAtCells { get; private set; }
        public int[,] CachedPointsAtCells { get; private set; }
        public bool IsGameOver { get; private set; }

        public CachedBaord(int score, int cachedScore, CachedTileType[,] tileTypes, CachedTileType[,] cachedTileTypes, int[,] pointsAtCells, int[,] cachedPointsAtCells, bool isGameOver)
        {
            Score = score;
            CachedScore = cachedScore;
            TileTypesAtCells = tileTypes;
            CachedTileTypesAtCells = cachedTileTypes;
            CachedPointsAtCells = cachedPointsAtCells;
            PointsAtCells = pointsAtCells;
            IsGameOver = isGameOver;
        }
    }
}
