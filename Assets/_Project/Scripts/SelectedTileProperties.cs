using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct SelectedTileProperties
{
    public GameObject TileObject { get; private set; }
    public Rigidbody TileRb { get; private set; }

    public SelectedTileProperties(GameObject tileObject, Rigidbody tileRb)
    {
        TileObject = tileObject;
        TileRb = tileRb;
    }
}