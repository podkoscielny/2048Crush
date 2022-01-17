using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Crush2048
{
    [System.Serializable]
    public struct TileProbabilityPair
    {
        [Range(0, 1)] public float spawnProbability;
        public TileType tileType;
    }
}
