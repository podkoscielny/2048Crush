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
    public static event Action<SelectedTile, SelectedTile> OnTilesMatch;

    [SerializeField] TextMeshPro tileText;
    [SerializeField] MeshRenderer tileRenderer;
    [SerializeField] BoxCollider tileCollider;
    [SerializeField] Outline outlineScript;
    [SerializeField] GridSystem gridSystem;
    [SerializeField] ObjectPool objectPool;
    [SerializeField] TileType[] tileTypes;

    public TileType TileType { get; private set; }

    private static SelectedTile _selectedTile;
    private static bool _canBeClicked = true;

    private int _pointsWorth;
    private Sequence _tileMoveSequence;
    private Vector3 _enlargedTileScale = new Vector3(0.4f, 0.4f, 0.4f);
    private Quaternion _initialRotation = new Quaternion(0, 0, 0, 0);
    private SelectedTile _emptyTileSelection = new SelectedTile();
    private Color _outlineColorGreen = new Color(0.5607f, 1f, 0.5647f);

    private const float CELL_SIZE_FACTOR = 0.8f;

    private void OnEnable() => InitializeTileType();
    private void OnDisable() => ResetProperties();
    private void Awake() => InitializeTileSize();

    private void OnMouseDown()
    {
        if (!_canBeClicked) return;

        SelectTile();
    }

    public static void EnableClick() => _canBeClicked = true;
    public static void DisableClick() => _canBeClicked = false;

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

        if (gridSystem.AreTilesClose(_selectedTile.TileCell, tileCell))
        {
            _canBeClicked = false;
            MatchTiles();
        }
        else
        {
            StartCoroutine(ResetOutlinesSelected(_selectedTile.OutlineScript));

            SetOutline(Color.red);
            _selectedTile.OutlineScript.OutlineColor = Color.red;
            _selectedTile = _emptyTileSelection;
        }
    }

    private void MatchTiles()
    {
        _tileMoveSequence = DOTween.Sequence().SetAutoKill(false);
        _tileMoveSequence.Append(_selectedTile.TileObject.transform.DODynamicLookAt(transform.position, 0.1f));
        _tileMoveSequence.Append(_selectedTile.TileObject.transform.DOMove(transform.position, 0.15f).SetEase(Ease.InBack));
        _tileMoveSequence.AppendCallback(MoveTileToPool);
        _tileMoveSequence.AppendCallback(SpawnMissingTile);
        _tileMoveSequence.AppendCallback(ReenableClick);
        _tileMoveSequence.Append(transform.DOPunchScale(_enlargedTileScale, 0.3f, 1));
    }

    private void MoveTileToPool() => objectPool.AddToPool(Tags.Tile, _selectedTile.TileObject);

    private void SpawnMissingTile()
    {
        Vector2Int firstCellInColumn = new Vector2Int(_selectedTile.TileCell.x, 0);
        Vector3 firstGridCellPosition = gridSystem.GridCells[_selectedTile.TileCell.x, 0];
        Vector3 spawnPosition = new Vector3(firstGridCellPosition.x, firstGridCellPosition.y + (gridSystem.CellHeight * 1.15f), transform.position.z);

        GameObject spawnedTile = objectPool.GetFromPool(Tags.Tile);
        spawnedTile.transform.position = spawnPosition;
        spawnedTile.transform.DOMoveY(firstGridCellPosition.y, 0.2f).SetDelay(0.1f);

        MoveTilesDown();

        gridSystem.AssignTileToCell(spawnedTile, firstCellInColumn);
    }

    private void MoveTilesDown()
    {
        int columnIndex = _selectedTile.TileCell.x;

        if (_selectedTile.TileCell.y == 0) return;

        for (int i = _selectedTile.TileCell.y - 1; i >= 0; i--)
        {
            Vector2Int nextGridCell = new Vector2Int(columnIndex, i + 1);
            float desiredPositionY = gridSystem.GridCells[columnIndex, i + 1].y;

            GameObject tileToBeMoved = gridSystem.TilesAtGridCells[columnIndex, i];
            tileToBeMoved.transform.DOMoveY(desiredPositionY, 0.2f);

            gridSystem.AssignTileToCell(tileToBeMoved, nextGridCell);
        }
    }

    private void ReenableClick()
    {
        _selectedTile = _emptyTileSelection;
        _canBeClicked = true;
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
