using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Crush2048
{
    public struct TileToCellPair
    {
        public GameObject Tile { get; private set; }
        public Vector2Int Cell { get; private set; }

        public TileToCellPair(GameObject tile, Vector2Int cell)
        {
            Tile = tile;
            Cell = cell;
        }
    }
}
