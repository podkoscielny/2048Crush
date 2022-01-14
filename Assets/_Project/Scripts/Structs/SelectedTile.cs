using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct SelectedTile
{
    public GameObject TileObject { get; private set; }
    public int PointsWorth { get; private set; }
    public Vector2Int TileCell { get; private set; }
    public TilePoints TilePoints { get; private set; }
    public bool IsSpecial { get; private set; }

    public SelectedTile(GameObject tileObject, int pointsWorth, Vector2Int tileCell, TilePoints tilePoints, bool isSpecial)
    {
        TileObject = tileObject;
        PointsWorth = pointsWorth;
        TileCell = tileCell;
        TilePoints = tilePoints;
        IsSpecial = isSpecial;
    }
}
