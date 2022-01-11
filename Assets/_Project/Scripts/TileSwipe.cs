using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class TileSwipe : MonoBehaviour
{
    public static event Action<SelectedTile, Transform, TilePoints> OnTilesMatch;

    [SerializeField] GridSystem gridSystem;
    [SerializeField] TilePoints tilePoints;

    public static SelectedTile selectedTile { get; private set; }

    private static SelectedTile _tileToBeSwipedInto;
    private static bool _isPointerDown = false;

    private SelectedTile _emptyTileSelection = new SelectedTile();

    private void OnEnable() => Board.OnTileMatchEnded += ClearSelectedTiles;

    private void OnDisable() => Board.OnTileMatchEnded -= ClearSelectedTiles;

    private void OnMouseDown()
    {
        if (!Board.CanTilesBeClicked) return;

        ClearSelectedTiles();
        _isPointerDown = true;

        Vector2Int tileCell = gridSystem.GetTileGridCell(gameObject);
        selectedTile = new SelectedTile(gameObject, tilePoints.PointsWorth, tileCell);
    }

    private void OnMouseUp()
    {
        _isPointerDown = false;
        ClearSelectedTiles();
    }

    private void OnMouseOver()
    {
        if (!CanTileBeSwiped()) return;

        Vector2Int tileCell = gridSystem.GetTileGridCell(gameObject);
        _tileToBeSwipedInto = new SelectedTile(gameObject, tilePoints.PointsWorth, tileCell);

        bool areTilesClose = gridSystem.AreTilesClose(selectedTile.TileCell, _tileToBeSwipedInto.TileCell);
        bool areTilesWorthSame = selectedTile.PointsWorth == _tileToBeSwipedInto.PointsWorth;

        if (areTilesClose && areTilesWorthSame)
        {
            OnTilesMatch?.Invoke(selectedTile, transform, tilePoints);
        }
        else
            SetNotMatchedTilesProperties();
    }

    private bool CanTileBeSwiped() => _isPointerDown && selectedTile.TileObject != null && selectedTile.TileObject != gameObject && _tileToBeSwipedInto.TileObject == null;

    private void SetNotMatchedTilesProperties()
    {
        Transform selectedTileTransform = selectedTile.TileObject.transform;
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

    private void ClearSelectedTiles() => selectedTile = _tileToBeSwipedInto = _emptyTileSelection;
}
