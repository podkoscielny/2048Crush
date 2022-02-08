using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Crush2048
{
    [System.Serializable]
    public struct CachedTileType
    {
        public int PointsWorth { get; private set; }
        public bool IsSpecial { get; private set; }
        public Behaviours TileBehaviour { get; private set; }

        public CachedTileType(int pointsWorth, bool isSpecial, Behaviours tileBehaviour)
        {
            PointsWorth = pointsWorth;
            IsSpecial = isSpecial;
            TileBehaviour = tileBehaviour;
        }
    }
}
