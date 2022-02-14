using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Crush2048
{
    [System.Serializable]
    public struct CachedTileType
    {
        public string Name { get; private set; }

        public CachedTileType(string name)
        {
            Name = name;
        }
    }
}
