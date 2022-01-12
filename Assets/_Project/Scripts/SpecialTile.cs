using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpecialTile : MonoBehaviour
{
    public static event Action<SelectedTile, SpecialTileBehaviour, GameObject> OnSpecialTileUsed;

    [SerializeField] MeshFilter meshFilter;
    [SerializeField] MeshRenderer meshRenderer;
    [SerializeField] BoxCollider tileCollider;
    [SerializeField] GameObject graphics;
    [SerializeField] GridSystem gridSystem;

    private bool _canBeInvoked = true;
    private SpecialTileBehaviour tileBehaviour;
    private SpecialTilePropability[] specialTiles;

    private const float CELL_SIZE_FACTOR = 0.8f;

    private void Awake()
    {
        CacheSpecialTiles();
        InitializeTileSize();
    }

    private void OnEnable()
    {
        ChangeTileProperties();
    }

    private void OnMouseOver()
    {
        if (AreSpecialTilesDisabled()) return;

        OnSpecialTileUsed?.Invoke(TileSwipe.selectedTile, tileBehaviour, gameObject);
        _canBeInvoked = false;
    }

    private void CacheSpecialTiles() => specialTiles = gridSystem.GridSize.SpecialTiles;

    private bool AreSpecialTilesDisabled() => TileSwipe.selectedTile.TileObject == null || !Board.CanTilesBeClicked || !_canBeInvoked;

    private void ChangeTileProperties()
    {
        int randomIndex = UnityEngine.Random.Range(0, specialTiles.Length);
        SpecialTileSO currentSpecialTile = specialTiles[randomIndex].specialTile;

        meshFilter.mesh = currentSpecialTile.SpecialTileMesh;
        meshRenderer.material = currentSpecialTile.SpecialTileMaterial;
        tileBehaviour = currentSpecialTile.SpecialTileBehaviour;
        graphics.transform.localPosition = currentSpecialTile.MeshOffset;
        graphics.transform.rotation = Quaternion.Euler(currentSpecialTile.MeshRotation);
    }

    private void InitializeTileSize()
    {
        float tileScaleX = gridSystem.CellWidth * CELL_SIZE_FACTOR;
        float tileScaleY = gridSystem.CellHeight * CELL_SIZE_FACTOR;
        float boxColliderFactor = 1 / CELL_SIZE_FACTOR;

        transform.localScale = new Vector3(tileScaleX, tileScaleY, transform.localScale.z);
        tileCollider.size = new Vector3(boxColliderFactor, boxColliderFactor, tileCollider.size.z);
    }
}
