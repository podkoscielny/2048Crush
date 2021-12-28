using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using Random = UnityEngine.Random;

public class TileSwipe : MonoBehaviour
{
    public event Action OnPointsUpdated;
    public static event Action<SelectedTile, Transform> OnTilesMatch;

    [SerializeField] GridSystem gridSystem;

    public int PointsWorth
    {
        get => _pointsWorth;

        private set
        {
            _pointsWorth = value;
            OnPointsUpdated?.Invoke();
        }
    }

    private static bool _isPointerDown = false;
    private static SelectedTile _selectedTile;
    private static SelectedTile _tileToBeSwipedInto;

    private int _pointsWorth = 2;
    private SelectedTile _emptyTileSelection = new SelectedTile();

    private void OnEnable()
    {
        Board.OnAssignPointsWorthToCells += AssignPointsWorthToCell;
        InitializePoints();
    }

    private void OnDisable() => Board.OnAssignPointsWorthToCells -= AssignPointsWorthToCell;

    private void OnMouseDown()
    {
        if (!Board.CanTilesBeClicked) return;

        _isPointerDown = true;

        Vector2Int tileCell = gridSystem.GetTileGridCell(gameObject);
        _selectedTile = new SelectedTile(gameObject, PointsWorth, tileCell);
    }

    private void OnMouseUp() => _isPointerDown = false;

    private void OnMouseOver()
    {
        if (!CanTileBeSwiped()) return;

        Vector2Int tileCell = gridSystem.GetTileGridCell(gameObject);
        _tileToBeSwipedInto = new SelectedTile(gameObject, PointsWorth, tileCell);

        bool areTilesClose = gridSystem.AreTilesClose(_selectedTile.TileCell, _tileToBeSwipedInto.TileCell);
        bool areTilesWorthSame = _selectedTile.PointsWorth == _tileToBeSwipedInto.PointsWorth;

        if (areTilesClose && areTilesWorthSame)
        {
            OnTilesMatch?.Invoke(_selectedTile, transform);
            StartCoroutine(UpdateTile());
        }
        else
            SetNotMatchedTilesProperties();
    }

    private IEnumerator UpdateTile()
    {
        yield return new WaitUntil(() => Board.CanTilesBeClicked);

        ClearSelectedTiles();
        PointsWorth *= 2;
    }

    private bool CanTileBeSwiped() => _isPointerDown && _selectedTile.TileObject != null && _selectedTile.TileObject != gameObject && _tileToBeSwipedInto.TileObject == null;

    private void SetNotMatchedTilesProperties()
    {
        Transform selectedTileTransform = _selectedTile.TileObject.transform;
        Transform tileToBeSwipedIntoTransform = _tileToBeSwipedInto.TileObject.transform;

        Quaternion rotation = GetDirectionOfRotation(selectedTileTransform.position, tileToBeSwipedIntoTransform.position);

        RotateTowards(selectedTileTransform, rotation);
        ClearSelectedTiles();
    }

    private Quaternion GetDirectionOfRotation(Vector3 positionOfObjectToBeRotated, Vector3 positionToBeLookedAt)
    {
        Vector3 direction = positionToBeLookedAt - positionOfObjectToBeRotated;
        Quaternion rotation = Quaternion.LookRotation(-direction, Vector3.up);

        return rotation;
    }

    private void RotateTowards(Transform selectedTileTransform, Quaternion rotation)
    {
        selectedTileTransform.DORotateQuaternion(rotation, 0.1f)
            .OnComplete(() => selectedTileTransform.DORotate(Vector3.zero, 0.1f).SetDelay(0.1f));
    }

    private void AssignPointsWorthToCell()
    {
        Vector2Int tileCell = gridSystem.GetTileGridCell(gameObject);
        gridSystem.AssignPointsWorthToCell(PointsWorth, tileCell);
    }

    private void InitializePoints() => PointsWorth = GetRandomPointsWorth();

    private int GetRandomPointsWorth()
    {
        TileProbabilityPair[] tileTypes = gridSystem.GridSize.TileTypes;
        float probabilitySum = gridSystem.GridSize.ProbabilitySum;
        float randomProbability = Random.Range(0, probabilitySum);
        float subtractFromSum = 0;

        for (int i = 0; i < tileTypes.Length; i++)
        {
            if (randomProbability - subtractFromSum <= tileTypes[i].probability) return tileTypes[i].pointsWorth;

            subtractFromSum -= tileTypes[i].probability;
        }

        return tileTypes[tileTypes.Length - 1].pointsWorth;
    }

    private void ClearSelectedTiles() => _selectedTile = _tileToBeSwipedInto = _emptyTileSelection;
}
