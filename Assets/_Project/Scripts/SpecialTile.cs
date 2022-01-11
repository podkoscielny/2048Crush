using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpecialTile : MonoBehaviour
{
    [SerializeField] MeshFilter meshFilter;
    [SerializeField] MeshRenderer meshRenderer;
    [SerializeField] GameObject graphics;
    [SerializeField] GridSystem gridSystem;

    private SpecialTilePropability[] specialTiles;

    private void Awake() => CacheSpecialTiles();

    private void OnEnable()
    {
        ChangeTileProperties();
    }

    private void OnMouseOver()
    {
        if (AreSpecialTilesDisabled()) return;
    }

    private void CacheSpecialTiles() => specialTiles = gridSystem.GridSize.SpecialTiles;

    private bool AreSpecialTilesDisabled() => TileSwipe._selectedTile.TileObject == null && !Board.CanTilesBeClicked;

    private void ChangeTileProperties()
    {
        int randomIndex = Random.Range(0, specialTiles.Length);
        SpecialTileSO currentSpecialTile = specialTiles[randomIndex].specialTile;

        meshFilter.mesh = currentSpecialTile.SpecialTileMesh;
        meshRenderer.material = currentSpecialTile.SpecialTileMaterial;
        graphics.transform.localPosition = currentSpecialTile.MeshOffset;
        graphics.transform.rotation = Quaternion.Euler(currentSpecialTile.MeshRotation);
    }
}
