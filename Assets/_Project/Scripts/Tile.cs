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
    [SerializeField] BoxCollider tileCollider;
    [SerializeField] Rigidbody tileRb;
    [SerializeField] GridSystem gridSystem;
    [SerializeField] TileType[] tileTypes;

    public TileType TileType { get; private set; }

    private const float CELL_SIZE_FACTOR = 0.85f; 

    private void OnEnable() => InitializeTileType();

    private void Awake() => InitializeTileSize();

    private void OnMouseDown()
    {
        SelectedTileProperties tileProperties = new SelectedTileProperties(gameObject, tileRb, TileType, this);
        OnTileClicked?.Invoke(tileProperties);
    }

    private void InitializeTileType()
    {
        int randomTileIndex = Random.Range(0, tileTypes.Length);
        TileType randomTile = tileTypes[randomTileIndex];

        tileText.text = randomTile.PointsToString;
        tileRenderer.material = randomTile.TileMaterial;

        TileType = randomTile;
    }

    private void InitializeTileSize()
    {
        float tileScaleX = gridSystem.CellWidth * CELL_SIZE_FACTOR;
        float tileScaleY = gridSystem.CellHeight * CELL_SIZE_FACTOR;
        float boxColliderFactor = 1 / CELL_SIZE_FACTOR;

        transform.localScale = new Vector3(tileScaleX, tileScaleY, transform.localScale.z);
        tileCollider.size = new Vector3(boxColliderFactor, boxColliderFactor, tileCollider.size.z);
    }
}