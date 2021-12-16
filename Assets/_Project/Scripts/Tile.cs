using System;
using UnityEngine;
using TMPro;
using Random = UnityEngine.Random;
using OutlineEffect;
using DG.Tweening;
using Tags = TagSystem.Tags;

public class Tile : MonoBehaviour
{
    public static event Action<SelectedTileProperties> OnTileClicked;

    [SerializeField] TextMeshPro tileText;
    [SerializeField] MeshRenderer tileRenderer;
    [SerializeField] BoxCollider tileCollider;
    [SerializeField] Rigidbody tileRb;
    [SerializeField] Outline outlineScript;
    [SerializeField] GridSystem gridSystem;
    [SerializeField] ObjectPool objectPool;
    [SerializeField] TileType[] tileTypes;

    public TileType TileType { get; private set; }

    private static SelectedTile _selectedTile;
    private static bool _canBeClicked = true;

    private int _pointsWorth;
    private Sequence _tileMoveSequence;
    private Vector3 _initialTileScale;
    private Vector3 _enlargedTileScale;
    private Quaternion _initialRotation = new Quaternion(0, 0, 0, 0);
    private SelectedTile _emptyTileSelection = new SelectedTile();
    private Color _outlineColorGreen = new Color(0.5607f, 1f, 0.5647f);

    private const float CELL_SIZE_FACTOR = 0.8f;
    private const float ENLARGED_TILE_SCALE = 1.2f;

    private void OnEnable() => InitializeTileType();

    private void OnDisable() => ResetProperties();

    private void Awake()
    {
        InitializeTileSize();
        CacheTileScales();
    }

    private void OnMouseDown()
    {
        if (!_canBeClicked) return;

        SelectTile();
    }

    private void SelectTile()
    {
        if (_selectedTile.TileObject == null)
        {
            Vector2Int tileCell = gridSystem.GetTileGridCell(gameObject);
            SelectedTile tileToBeSelected = new SelectedTile(gameObject, _pointsWorth, tileCell);
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

        if (gridSystem.AreTilesClose(_selectedTile.TileCell, tileCell, out Axis closeInAxis))
        {
            _canBeClicked = false;
            MoveTile(closeInAxis);
        }
        else
        {

        }
    }

    private void MoveTile(Axis closeInAxis)
    {
        _tileMoveSequence = DOTween.Sequence().SetAutoKill(false);
        _tileMoveSequence.Append(_selectedTile.TileObject.transform.DORotateQuaternion(GetTileRotation(closeInAxis), 0.1f));
        _tileMoveSequence.Append(MoveTileInDirection(closeInAxis).SetEase(Ease.InBack));
        _tileMoveSequence.AppendCallback(MoveTileToPool);
        _tileMoveSequence.Append(transform.DOScale(_enlargedTileScale, 0.2f));
        _tileMoveSequence.Append(transform.DOScale(_initialTileScale, 0.2f));
    }

    private void MoveTileToPool() => objectPool.AddToPool(Tags.Tile, _selectedTile.TileObject);

    private Quaternion GetTileRotation(Axis closeInAxis)
    {
        Quaternion rotation;

        if (closeInAxis == Axis.X)
        {
            rotation = transform.position.x < _selectedTile.TileObject.transform.position.x ? Quaternion.Euler(0, 67, 0) : Quaternion.Euler(0, -67, 0);
        }
        else
        {
            rotation = transform.position.y < _selectedTile.TileObject.transform.position.y ? Quaternion.Euler(-53, 0, 0) : Quaternion.Euler(53, 0, 0);
        }

        return rotation;
    }

    private Tween MoveTileInDirection(Axis closeInAxis)
    {
        if (closeInAxis == Axis.X)
        {
            return _selectedTile.TileObject.transform.DOMoveX(transform.position.x, 0.15f).SetEase(Ease.InBack);
        }
        else
        {
            return _selectedTile.TileObject.transform.DOMoveY(transform.position.y, 0.15f).SetEase(Ease.InBack);
        }
    }

    private void SetOutline(Color outlineColor)
    {
        outlineScript.enabled = true;
        outlineScript.OutlineColor = outlineColor;
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

    private void CacheTileScales()
    {
        _initialTileScale = transform.localScale;
        _enlargedTileScale = _initialTileScale * ENLARGED_TILE_SCALE;
    }

    private void ResetProperties()
    {
        transform.rotation = _initialRotation;
        outlineScript.enabled = false;
    }

    private struct SelectedTile
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
}