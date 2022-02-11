using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Crush2048
{
    [System.Serializable]
    public struct CachedBoard
    {
        public int score;
        public int cachedScore;
        public int moveReversesLeft;
        public CachedTileType[,] tileTypesAtCells;
        public CachedTileType[,] cachedTileTypesAtCells;
        public int[,] pointsAtCells;
        public int[,] cachedPointsAtCells;
        public bool isGameOver;
    }
}
