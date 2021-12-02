using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Random = UnityEngine.Random;

public class Tile : MonoBehaviour
{
    public static event Action<List<SelectedTileProperties>> OnTileSelected;

    [SerializeField] TextMeshPro tileText;
    [SerializeField] MeshRenderer tileRenderer;
    [SerializeField] Rigidbody tileRb;
    [SerializeField] TileType[] tileTypes;

    private static readonly List<SelectedTileProperties> _selectedTiles = new List<SelectedTileProperties>();

    void OnEnable()
    {
        int randomTileIndex = Random.Range(0, tileTypes.Length);
        TileType randomTile = tileTypes[randomTileIndex];

        tileText.text = randomTile.PointsToString;
        tileRenderer.material = randomTile.TileMaterial;
    }

    void OnMouseDown()
    {
        if (_selectedTiles.Count == 0)
        {
            AddTileToSelectedTiles();
        }
        else if (_selectedTiles.Count == 1 && !(_selectedTiles[0].TileObject == gameObject))
        {
            AddTileToSelectedTiles();
            OnTileSelected?.Invoke(_selectedTiles);
            _selectedTiles.Clear();
        }
    }

    private void AddTileToSelectedTiles()
    {
        SelectedTileProperties tileProperties = new SelectedTileProperties(gameObject, tileRb);

        _selectedTiles.Add(tileProperties);
    }
}