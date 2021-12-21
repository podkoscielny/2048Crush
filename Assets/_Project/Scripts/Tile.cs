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

    private Camera _mainCamera;
    private Vector3 _mouseclickWorldPosition;
    private int _pointsWorth = 2;
    private bool _isGoingToBeUpdated = false;
    private Quaternion _initialRotation = new Quaternion(0, 0, 0, 0);
    private SelectedTile _emptyTileSelection = new SelectedTile();

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

        Vector2Int tileCell = gridSystem.GetTileGridCell(gameObject);
        _selectedTile = new SelectedTile(gameObject, _pointsWorth, tileCell, outlineScript);
        _mouseclickWorldPosition = _mainCamera.ScreenToWorldPoint(Input.mousePosition);
    }

    private void OnMouseUp()
    {
        if (!_isGoingToBeUpdated && !_canTilesBeClicked) _canTilesBeClicked = true;
    }

    private void OnMouseDrag()
    {
        if (!_canTilesBeClicked) return;

        DetectSwipeDirection();
    }

    private void DetectSwipeDirection()
    {
        Vector3 mousePositionToWorld = _mainCamera.ScreenToWorldPoint(Input.mousePosition);

        if (Vector3.Distance(_mouseclickWorldPosition, mousePositionToWorld) <= MOUSE_DRAG_DISTANCE_TO_MOVE) return;

        Vector2Int tileCellDirection = GetSwipeDirection();
        int tileCellCoordinateX = _selectedTile.TileCell.x + tileCellDirection.x;
        int tileCellCoordinateY = _selectedTile.TileCell.y + tileCellDirection.y;

        if (tileCellCoordinateX < 0 || tileCellCoordinateY < 0 || tileCellCoordinateX >= gridSystem.GridCells.GetLength(0) || tileCellCoordinateY >= gridSystem.GridCells.GetLength(1))
            PreventClickFromNotMatchedTiles();
        else
        {
            GameObject tileToBeUpdated = gridSystem.TilesAtGridCells[tileCellCoordinateX, tileCellCoordinateY];
            MatchTiles(tileToBeUpdated);
        }
    }

    private Vector2Int GetSwipeDirection()
    {
        Vector3 mousePositionToWorld = _mainCamera.ScreenToWorldPoint(Input.mousePosition);
        Vector3 mousePositionDifference = mousePositionToWorld - _mouseclickWorldPosition;

        Vector2Int tileCellToBeSwiped = Vector2Int.zero;

        if (Mathf.Abs(mousePositionDifference.x) > Mathf.Abs(mousePositionDifference.y))
        {
            tileCellToBeSwiped.y = 0;
            tileCellToBeSwiped.x = mousePositionDifference.x > 0 ? 1 : -1;
        }
        else
        {
            tileCellToBeSwiped.x = 0;
            tileCellToBeSwiped.y = mousePositionDifference.y > 0 ? -1 : 1;
        }

        return tileCellToBeSwiped;
    }

    private void MatchTiles(GameObject tileToBeUpdated)
    {
        if (_pointsWorth == tileToBeUpdated.GetComponent<Tile>()._pointsWorth)
        {
            _canTilesBeClicked = false;
            tileToBeUpdated.GetComponent<Tile>()._isGoingToBeUpdated = true;
            OnTilesMatch?.Invoke(_selectedTile, tileToBeUpdated.transform);
        }
        else
            PreventClickFromNotMatchedTiles();
    }

    private void PreventClickFromNotMatchedTiles()
    {
        _canTilesBeClicked = false;
        SetOutline(Color.red);
        StartCoroutine(ResetOutlinesSelected());
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

    private void SetOutline(Color outlineColor)
    {
        outlineScript.enabled = true;
        outlineScript.OutlineColor = outlineColor;
    }

    private IEnumerator ResetOutlinesSelected()
    {
        yield return new WaitForSeconds(0.3f);

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
