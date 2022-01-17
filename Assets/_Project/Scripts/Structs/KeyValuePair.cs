using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Crush2048
{
    [System.Serializable]
    public struct KeyValuePair<TKey, TValue>
    {
        public TKey key;
        public TValue value;
    }
}
