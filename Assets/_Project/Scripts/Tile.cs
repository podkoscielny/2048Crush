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
    [SerializeField] Outline outlineScript;
    [SerializeField] GridSystem gridSystem;
    [SerializeField] TileType[] tileTypes;

    public TileType TileType { get; private set; }

    private int _pointsWorth;
    private static SelectedTile _selectedTile;
    private SelectedTile _emptyTileSelection = new SelectedTile();
    private Color _outlineColorGreen = new Color(0.5607f, 1f, 0.5647f);

    private const float CELL_SIZE_FACTOR = 0.8f;

    private void OnEnable() => InitializeTileType();

    private void Awake() => InitializeTileSize();

    private void OnMouseDown()
    {
        SelectedTileProperties tileProperties = new SelectedTileProperties(gameObject, tileRb, TileType, this);
        OnTileClicked?.Invoke(tileProperties);
        SelectTile();
    }

    private void SelectTile()
    {
        if (_selectedTile.TileObject == null)
        {
            SelectedTile tileToBeSelected = new SelectedTile(gameObject, _pointsWorth);
            _selectedTile = tileToBeSelected;
            SetOutline();
        }
        else
        {
            if (_selectedTile.TileObject == gameObject)
            {
                DeselectTile();
            }
            else
            {

            }
        }
    }

    private void SetOutline()
    {
        outlineScript.enabled = true;
        outlineScript.OutlineColor = _outlineColorGreen;
    }

    private void DeselectTile()
    {
        _selectedTile = _emptyTileSelection;
        outlineScript.enabled = false;
    }

    private void InitializeTileType()
    {
        int randomTileIndex = Random.Range(0, tileTypes.Length);
        TileType randomTile = tileTypes[randomTileIndex];

        _pointsWorth = randomTile.PointsWorth;
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

    private struct SelectedTile
    {
        public GameObject TileObject { get; private set; }
        public int PointsWorth { get; private set; }

        public SelectedTile(GameObject tileObject, int pointsWorth)
        {
            TileObject = tileObject;
            PointsWorth = pointsWorth;
        }
    }
}