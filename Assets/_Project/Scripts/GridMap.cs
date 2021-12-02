using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridMap : MonoBehaviour
{
    [SerializeField] GridSystem gridSystem;
    [SerializeField] MeshRenderer gridRenderer;

    private Vector3[,] gridCells;
    private Vector3 _cubeSize = new Vector3(0.5f, 0.5f, 0.5f);
    private float _cellWidth;
    private float _cellHeight;

    void OnEnable()
    {
        Tile.OnTileSelected += HandleSwitchTiles;
    }

    void OnDisable()
    {
        Tile.OnTileSelected -= HandleSwitchTiles;
    }

    void Start()
    {
        gridCells = new Vector3[gridSystem.GridSize.Rows, gridSystem.GridSize.Columns];
        InitializeGrid();
    }

    void OnDrawGizmos()
    {
        if (gridCells != null)
        {
            for (int i = 0; i < gridCells.GetLength(0); i++)
            {
                for (int j = 0; j < gridCells.GetLength(1); j++)
                {
                    Gizmos.DrawWireCube(gridCells[i, j], _cubeSize);
                }
            }
        }
    }

    private void HandleSwitchTiles(List<SelectedTileProperties> selectedTiles)
    {
        if (Vector3.Distance(selectedTiles[0].TileRb.position, selectedTiles[1].TileRb.position) <= _cellWidth)
        {
            Debug.Log("True");
        }
        else
        {
            Debug.Log("False");
        }
    }

    private void InitializeGrid()
    {
        int rowsCount = gridSystem.GridSize.Rows;
        int columnsCount = gridSystem.GridSize.Columns;

        float gridWidth = gridRenderer.bounds.size.x;
        float gridHeight = gridRenderer.bounds.size.y;
        Vector3 gridCenter = gridRenderer.bounds.center;

        float cellWidth = gridWidth / rowsCount;
        float cellHeight = gridHeight / columnsCount;

        _cellWidth = cellWidth;
        _cellHeight = cellHeight;

        _cubeSize = new Vector3(cellWidth, cellHeight, 0.1f);

        for (int i = 0; i < rowsCount; i++)
        {
            float xPosition = gridCenter.x - (gridWidth / 2) + (cellWidth / 2) + (i * cellWidth);

            for (int j = 0; j < columnsCount; j++)
            {
                float yPosition = gridCenter.y - (gridHeight / 2) + (cellHeight / 2) + (j * cellHeight);

                gridCells[i, j] = new Vector3(xPosition, yPosition, gridCenter.z);
            }
        }
    }
}