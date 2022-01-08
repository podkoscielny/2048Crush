using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class TileSwipe : MonoBehaviour
{
    public static event Action<SelectedTile, Transform> OnTilesMatch;

    [SerializeField] GridSystem gridSystem;
    [SerializeField] TilePoints tilePoints;

    private static bool _isPointerDown = false;
    private static SelectedTile _selectedTile;
    private static SelectedTile _tileToBeSwipedInto;

    private SelectedTile _emptyTileSelection = new SelectedTile();

    private void OnMouseDown()
    {
        if (!Board.CanTilesBeClicked) return;

        _isPointerDown = true;

        Vector2Int tileCell = gridSystem.GetTileGridCell(gameObject);
        _selectedTile = new SelectedTile(gameObject, tilePoints.PointsWorth, tileCell);
    }

    private void OnMouseUp() => _isPointerDown = false;

    private void OnMouseOver()
    {
        if (!CanTileBeSwiped()) return;

        Vector2Int tileCell = gridSystem.GetTileGridCell(gameObject);
        _tileToBeSwipedInto = new SelectedTile(gameObject, tilePoints.PointsWorth, tileCell);

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
        tilePoints.UpdatePoints(2);
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

    private void ClearSelectedTiles() => _selectedTile = _tileToBeSwipedInto = _emptyTileSelection;
}
