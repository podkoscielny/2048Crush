using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

namespace Crush2048
{
    public class TileSwipe : MonoBehaviour
    {
        public static event Action<SelectedTile, SelectedTile, BehaviourDelegate> OnTilesMatch;

        [SerializeField] GridSystem gridSystem;
        [SerializeField] TilePoints tilePoints;
        [SerializeField] TileBehaviour tileBehaviour;

        public static SelectedTile selectedTile { get; private set; }

        private static SelectedTile _tileToBeSwipedInto;
        private static bool _isPointerDown = false;

        private SelectedTile _emptyTileSelection = new SelectedTile();

        private void OnEnable() => TileMatchSequence.OnTileMatchEnded += ClearSelectedTiles;

        private void OnDisable() => TileMatchSequence.OnTileMatchEnded -= ClearSelectedTiles;

        private void OnMouseDown()
        {
            if (!TileMatchSequence.CanTilesBeClicked) return;

            ClearSelectedTiles();
            _isPointerDown = true;

            Vector2Int tileCell = gridSystem.GetTileGridCell(gameObject);
            selectedTile = new SelectedTile(gameObject, tileCell, tilePoints, tileBehaviour);
        }

        private void OnMouseUp()
        {
            _isPointerDown = false;
            ClearSelectedTiles();
        }

        private void OnMouseOver()
        {
            if (!CanTileBeSwiped()) return;

            CheckTilesMatch();
        }

        private void CheckTilesMatch()
        {
            Vector2Int tileCell = gridSystem.GetTileGridCell(gameObject);
            _tileToBeSwipedInto = new SelectedTile(gameObject, tileCell, tilePoints, tileBehaviour);

            bool areTilesClose = gridSystem.AreTilesClose(selectedTile.TileCell, _tileToBeSwipedInto.TileCell);
            bool areTilesWorthSame = selectedTile.TilePoints.PointsWorth == _tileToBeSwipedInto.TilePoints.PointsWorth;
            bool isTileSpecial = tileBehaviour.IsSpecial;
            bool isSelectedTileSpecial = selectedTile.TileBehaviour.IsSpecial;

            if (areTilesClose)
            {
                if (!isTileSpecial && !isSelectedTileSpecial && areTilesWorthSame)
                {
                    OnTilesMatch?.Invoke(selectedTile, _tileToBeSwipedInto, tileBehaviour.Behaviour);
                }
                else if (isTileSpecial && !isSelectedTileSpecial)
                {
                    OnTilesMatch?.Invoke(selectedTile, _tileToBeSwipedInto, tileBehaviour.Behaviour);
                }
                else if (!isTileSpecial && isSelectedTileSpecial)
                {
                    OnTilesMatch?.Invoke(selectedTile, _tileToBeSwipedInto, selectedTile.TileBehaviour.Behaviour);
                }
                else
                    SetNotMatchedTilesProperties();
            }
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
}
