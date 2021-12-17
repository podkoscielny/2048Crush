using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using OutlineEffect;

public struct SelectedTile
{
    public GameObject TileObject { get; private set; }
    public int PointsWorth { get; private set; }
    public Vector2Int TileCell { get; private set; }
    public Outline OutlineScript { get; private set; }

    public SelectedTile(GameObject tileObject, int pointsWorth, Vector2Int tileCell, Outline outlineScript)
    {
        TileObject = tileObject;
        PointsWorth = pointsWorth;
        TileCell = tileCell;
        OutlineScript = outlineScript;
    }
}
