using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Random = UnityEngine.Random;
using OutlineEffect;

public class Tile : MonoBehaviour
{
    public static event Action<SelectedTile, Transform> OnTilesMatch;

    [SerializeField] TextMeshPro tileText;
    [SerializeField] TextMeshPro backgroundText;
    [SerializeField] BoxCollider tileCollider;
    [SerializeField] Outline outlineScript;
    [SerializeField] GridSystem gridSystem;
    [SerializeField] Gradient tileBackgroundGradient;

    private static SelectedTile _selectedTile;
    private static bool _canTilesBeClicked = true;

    private int _pointsWorth = 2;
    private Camera _mainCamera;
    private bool _isGoingToBeUpdated = false;
    private Vector3 _mouseclickWorldPosition;
    private Quaternion _initialRotation = new Quaternion(0, 0, 0, 0);
    private SelectedTile _emptyTileSelection = new SelectedTile();
    private Color _outlineColorGreen = new Color(0.5607f, 1f, 0.5647f);

    private const float MOUSE_DRAG_DISTANCE_TO_MOVE = 0.6f;
    private const float CELL_SIZE_FACTOR = 0.8f;
    private const int MAXED_COLOR_AT_TWO_TO_THE_POWER = 13;

    private void OnEnable()
    {
        Board.OnTileSequenceEnded += UpdateMergedTile;
        InitializeTileType();
    }

    private void OnDisable()
    {
        Board.OnTileSequenceEnded -= UpdateMergedTile;
        ResetProperties();
    }

    private void Awake()
    {
        _mainCamera = Camera.main;
        InitializeTileSize();
    }

    private void OnMouseDown()
    {
        if (!_canTilesBeClicked) return;

        SelectTile();
        _mouseclickWorldPosition = _mainCamera.ScreenToWorldPoint(Input.mousePosition);
    }

    private void OnMouseDrag()
    {
        if (!_canTilesBeClicked) return;

        MoveTileInDirection();
    }

    private void MoveTileInDirection()
    {
        Vector3 mousePositionToWorld = _mainCamera.ScreenToWorldPoint(Input.mousePosition);

        if (mousePositionToWorld.x - _mouseclickWorldPosition.x >= MOUSE_DRAG_DISTANCE_TO_MOVE && _selectedTile.TileCell.x != gridSystem.GridCells.GetLength(0) - 1)
        {
            if (_pointsWorth == gridSystem.TilesAtGridCells[_selectedTile.TileCell.x + 1, _selectedTile.TileCell.y].GetComponent<Tile>()._pointsWorth)
            {
                _canTilesBeClicked = false;
                gridSystem.TilesAtGridCells[_selectedTile.TileCell.x + 1, _selectedTile.TileCell.y].GetComponent<Tile>()._isGoingToBeUpdated = true;
                OnTilesMatch?.Invoke(_selectedTile, gridSystem.TilesAtGridCells[_selectedTile.TileCell.x + 1, _selectedTile.TileCell.y].transform);
            }
        }
        else if (mousePositionToWorld.x - _mouseclickWorldPosition.x <= -MOUSE_DRAG_DISTANCE_TO_MOVE && _selectedTile.TileCell.x != 0)
        {
            if (_pointsWorth == gridSystem.TilesAtGridCells[_selectedTile.TileCell.x - 1, _selectedTile.TileCell.y].GetComponent<Tile>()._pointsWorth)
            {
                _canTilesBeClicked = false;
                gridSystem.TilesAtGridCells[_selectedTile.TileCell.x - 1, _selectedTile.TileCell.y].GetComponent<Tile>()._isGoingToBeUpdated = true;
                OnTilesMatch?.Invoke(_selectedTile, gridSystem.TilesAtGridCells[_selectedTile.TileCell.x - 1, _selectedTile.TileCell.y].transform);
            }
        }
        else if (mousePositionToWorld.y - _mouseclickWorldPosition.y >= MOUSE_DRAG_DISTANCE_TO_MOVE && _selectedTile.TileCell.y != gridSystem.GridCells.GetLength(1) - 1)
        {
            if (_pointsWorth == gridSystem.TilesAtGridCells[_selectedTile.TileCell.x, _selectedTile.TileCell.y - 1].GetComponent<Tile>()._pointsWorth)
            {
                _canTilesBeClicked = false;
                gridSystem.TilesAtGridCells[_selectedTile.TileCell.x, _selectedTile.TileCell.y - 1].GetComponent<Tile>()._isGoingToBeUpdated = true;
                OnTilesMatch?.Invoke(_selectedTile, gridSystem.TilesAtGridCells[_selectedTile.TileCell.x, _selectedTile.TileCell.y - 1].transform);
            }
        }
        else if (mousePositionToWorld.y - _mouseclickWorldPosition.y <= -MOUSE_DRAG_DISTANCE_TO_MOVE && _selectedTile.TileCell.y != 0)
        {
            if (_pointsWorth == gridSystem.TilesAtGridCells[_selectedTile.TileCell.x, _selectedTile.TileCell.y + 1].GetComponent<Tile>()._pointsWorth)
            {
                _canTilesBeClicked = false;
                gridSystem.TilesAtGridCells[_selectedTile.TileCell.x, _selectedTile.TileCell.y + 1].GetComponent<Tile>()._isGoingToBeUpdated = true;
                OnTilesMatch?.Invoke(_selectedTile, gridSystem.TilesAtGridCells[_selectedTile.TileCell.x, _selectedTile.TileCell.y + 1].transform);
            }
        }
    }

    private void UpdateMergedTile()
    {
        if (!_isGoingToBeUpdated) return;

        _selectedTile = _emptyTileSelection;

        _canTilesBeClicked = true;
        _isGoingToBeUpdated = false;
        _pointsWorth *= 2;

        UpdateTileText();
        SetBackgroundColor();
    }
    private void UpdateTileText() => tileText.text = _pointsWorth.ToString();

    private void SelectTile()
    {
        if (_selectedTile.TileObject == null)
        {
            Vector2Int tileCell = gridSystem.GetTileGridCell(gameObject);
            _selectedTile = new SelectedTile(gameObject, _pointsWorth, tileCell, outlineScript);

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

    private void DeselectTile()
    {
        _selectedTile = _emptyTileSelection;
        outlineScript.enabled = false;
    }

    private void InitializeTileType()
    {
        TileType randomTile = GetRandomTileType();

        _pointsWorth = randomTile.PointsWorth;
        tileText.text = randomTile.PointsToString;
        transform.rotation = _initialRotation;

        SetBackgroundColor();
    }

    private void SetBackgroundColor()
    {
        int pointsAsTwoToThePower = PointsAsTwoToThePower();

        float colorGradientPercentage = Mathf.Min((float)pointsAsTwoToThePower / (float)MAXED_COLOR_AT_TWO_TO_THE_POWER, 1);
        backgroundText.color = tileBackgroundGradient.Evaluate(colorGradientPercentage);
    }

    private int PointsAsTwoToThePower()
    {
        int pointsAsTwoToThePower = _pointsWorth;
        int powers = 0;

        do
        {
            pointsAsTwoToThePower /= 2;
            powers++;

        } while (pointsAsTwoToThePower != 1);

        return powers;
    }

    private TileType GetRandomTileType()
    {
        TileProbabilityPair[] tileTypes = gridSystem.GridSize.TileTypes;
        float probabilitySum = gridSystem.GridSize.ProbabilitySum;
        float randomProbability = Random.Range(0, probabilitySum);
        float subtractFromSum = 0;

        for (int i = 0; i < tileTypes.Length; i++)
        {
            if (randomProbability - subtractFromSum <= tileTypes[i].probability) return tileTypes[i].tileType;

            subtractFromSum -= tileTypes[i].probability;
        }

        return tileTypes[tileTypes.Length - 1].tileType;
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
