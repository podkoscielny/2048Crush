using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct SelectedTile
{
    public GameObject TileObject { get; private set; }
    public Vector2Int TileCell { get; private set; }
    public TilePoints TilePoints { get; private set; }
    public TileBehaviour TileBehaviour { get; private set; }

    public SelectedTile(GameObject tileObject, Vector2Int tileCell, TilePoints tilePoints, TileBehaviour tileBehaviour)
    {
        TileObject = tileObject;
        TileCell = tileCell;
        TilePoints = tilePoints;
        TileBehaviour = tileBehaviour;
    }
}
