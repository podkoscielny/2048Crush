using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Tags = TagSystem.Tags;

public class PooledObject : MonoBehaviour
{
    public Tags PoolTag { get; private set; }

    public void SetPoolTag(Tags tag) => PoolTag = tag;
}