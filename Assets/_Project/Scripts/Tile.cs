using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Tile : MonoBehaviour
{
    [SerializeField] TextMeshPro tileText;
    [SerializeField] MeshRenderer tileRenderer;
    [SerializeField] TileType[] tileTypes;

    void OnEnable()
    {
        int randomTileIndex = Random.Range(0, tileTypes.Length);
        TileType randomTile = tileTypes[randomTileIndex];

        tileText.text = randomTile.PointsToString;
        tileRenderer.material = randomTile.TileMaterial;
    }

    private void OnMouseDown()
    {
        Debug.Log(gameObject.name);
    }
}