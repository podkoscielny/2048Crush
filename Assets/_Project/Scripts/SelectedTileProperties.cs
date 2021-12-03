using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct SelectedTileProperties
{
    public GameObject TileObject { get; private set; }
    public Rigidbody TileRb { get; private set; }
    public TileType TileType { get; private set; }
    public Tile TileScript { get; private set; }

    public SelectedTileProperties(GameObject tileObject, Rigidbody tileRb, TileType tileType, Tile tileScript)
    {
        TileObject = tileObject;
        TileRb = tileRb;
        TileType = tileType;
        TileScript = tileScript;
    }
}