using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Tags = TagSystem.TagSystem.Tags;

namespace Crush2048
{
    public class PooledObject : MonoBehaviour
    {
        public Tags PoolTag { get; private set; }

        public void SetPoolTag(Tags tag) => PoolTag = tag;
    }
}