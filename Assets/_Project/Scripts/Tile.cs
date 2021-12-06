using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Random = UnityEngine.Random;

public class Tile : MonoBehaviour
{
    public static event Action<SelectedTileProperties> OnTileClicked;

    [SerializeField] TextMeshPro tileText;
    [SerializeField] MeshRenderer tileRenderer;
    [SerializeField] Rigidbody tileRb;
    [SerializeField] GridSystem gridSystem;
    [SerializeField] TileType[] tileTypes;

    public TileType TileType { get; private set; }

    private void OnEnable()
    {
        int randomTileIndex = Random.Range(0, tileTypes.Length);
        TileType randomTile = tileTypes[randomTileIndex];

        tileText.text = randomTile.PointsToString;
        tileRenderer.material = randomTile.TileMaterial;

        TileType = randomTile;
    }

    private void Awake()
    {
        float tileScaleX = gridSystem.CellWidth * 0.9f;
        float tileScaleY = gridSystem.CellHeight * 0.9f;

        transform.localScale = new Vector3(tileScaleX, tileScaleY, transform.localScale.z);
    }

    private void OnMouseDown()
    {
        SelectedTileProperties tileProperties = new SelectedTileProperties(gameObject, tileRb, TileType, this);
        OnTileClicked?.Invoke(tileProperties);
    }
}