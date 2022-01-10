using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpecialTile : MonoBehaviour
{
    [SerializeField] MeshFilter meshFilter;
    [SerializeField] MeshRenderer meshRenderer;
    [SerializeField] GameObject graphics;
    [SerializeField] SpecialTileSO[] specialTiles;

    private int _currentIndex = 0;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            ChangeTileProperties();
        }
    }

    private void OnMouseOver()
    {
        if (AreSpecialTilesDisabled()) return;
    }

    private bool AreSpecialTilesDisabled() => TileSwipe._selectedTile.TileObject == null && !Board.CanTilesBeClicked;

    private void ChangeTileProperties()
    {
        _currentIndex = _currentIndex + 1 >= specialTiles.Length ? 0 : _currentIndex + 1;

        SpecialTileSO currentSpecialTile = specialTiles[_currentIndex];

        meshFilter.mesh = currentSpecialTile.SpecialTileMesh;
        meshRenderer.material = currentSpecialTile.SpecialTileMaterial;
        graphics.transform.localPosition = currentSpecialTile.MeshOffset;
        graphics.transform.rotation = Quaternion.Euler(currentSpecialTile.MeshRotation);
    }
}
