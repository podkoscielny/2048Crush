using System;
using System.Collections;
using UnityEngine;
using TMPro;
using Random = UnityEngine.Random;
using OutlineEffect;
using DG.Tweening;
using Tags = TagSystem.Tags;

public class Tile : MonoBehaviour
{
    public static event Action<SelectedTile, Transform> OnTilesMatch;

    [SerializeField] TextMeshPro tileText;
    [SerializeField] MeshRenderer tileRenderer;
    [SerializeField] BoxCollider tileCollider;
    [SerializeField] Outline outlineScript;
    [SerializeField] GridSystem gridSystem;
    [SerializeField] TileType[] tileTypes;

    public TileType TileType { get; private set; }

    private static bool _canTilesBeClicked = true;
    private static SelectedTile _selectedTile;

    private int _pointsWorth;
    private bool _isGoingToBeUpdated = false;
    private Quaternion _initialRotation = new Quaternion(0, 0, 0, 0);
    private SelectedTile _emptyTileSelection = new SelectedTile();
    private Color _outlineColorGreen = new Color(0.5607f, 1f, 0.5647f);

    private const float CELL_SIZE_FACTOR = 0.8f;

    private void OnEnable()
    {
        Board.OnTileSequenceEnded += ReenableClick;
        InitializeTileType();
    }

    private void OnDisable()
    {
        Board.OnTileSequenceEnded -= ReenableClick;
        ResetProperties();
    }

    private void Awake() => InitializeTileSize();

    private void OnMouseDown()
    {
        if (!_canTilesBeClicked) return;

        SelectTile();
    }

    private void SelectTile()
    {
        if (_selectedTile.TileObject == null)
        {
            Vector2Int tileCell = gridSystem.GetTileGridCell(gameObject);
            SelectedTile tileToBeSelected = new SelectedTile(gameObject, _pointsWorth, tileCell, outlineScript);
            _selectedTile = tileToBeSelected;

            SetOutline(_outlineColorGreen);
        }
        else
        {
            if (_selectedTile.TileObject == gameObject)
            {
                DeselectTile();
            }
            else
            {
                CheckMatch();
            }
        }
    }

    private void CheckMatch()
    {
        Vector2Int tileCell = gridSystem.GetTileGridCell(gameObject);

        if (gridSystem.AreTilesClose(_selectedTile.TileCell, tileCell) && _selectedTile.PointsWorth == _pointsWorth)
        {
            _canTilesBeClicked = false;
            _isGoingToBeUpdated = true;
            OnTilesMatch?.Invoke(_selectedTile, transform);
        }
        else
        {
            StartCoroutine(ResetOutlinesSelected(_selectedTile.OutlineScript));

            SetOutline(Color.red);
            _selectedTile.OutlineScript.OutlineColor = Color.red;
            _selectedTile = _emptyTileSelection;
        }
    }

    private void SetOutline(Color outlineColor)
    {
        outlineScript.enabled = true;
        outlineScript.OutlineColor = outlineColor;
    }

    private IEnumerator ResetOutlinesSelected(Outline selectedTileOutline)
    {
        yield return new WaitForSeconds(0.3f);

        if (outlineScript.OutlineColor != _outlineColorGreen) outlineScript.enabled = false;
        if (selectedTileOutline.OutlineColor != _outlineColorGreen) selectedTileOutline.enabled = false;
    }

    private void ReenableClick()
    {
        _selectedTile = _emptyTileSelection;

        if (_isGoingToBeUpdated)
        {
            _canTilesBeClicked = true;
            _isGoingToBeUpdated = false;
            _pointsWorth *= 2;

            UpdateTileText();
        }
    }

    private void DeselectTile()
    {
        _selectedTile = _emptyTileSelection;
        outlineScript.enabled = false;
    }

    private void UpdateTileText() => tileText.text = _pointsWorth.ToString();

    private void InitializeTileType()
    {
        int randomTileIndex = Random.Range(0, tileTypes.Length);
        TileType randomTile = tileTypes[randomTileIndex];

        _pointsWorth = randomTile.PointsWorth;
        tileText.text = randomTile.PointsToString;
        tileRenderer.material = randomTile.TileMaterial;
        transform.rotation = _initialRotation;

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

    private void ResetProperties()
    {
        transform.rotation = _initialRotation;
        outlineScript.enabled = false;
    }
}
