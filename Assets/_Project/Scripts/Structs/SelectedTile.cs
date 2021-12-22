using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct SelectedTile
{
    public GameObject TileObject { get; private set; }
    public int PointsWorth { get; private set; }
    public Vector2Int TileCell { get; private set; }

    public SelectedTile(GameObject tileObject, int pointsWorth, Vector2Int tileCell)
    {
        TileObject = tileObject;
        PointsWorth = pointsWorth;
        TileCell = tileCell;
    }
}
