using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

[CreateAssetMenu(fileName = "GridSystem", menuName = "ScriptableObjects/GridSystem")]
public class GridSystem : ScriptableObject
{
    [SerializeField] GridSize gridSize;

    public float CellWidth => _cellWidth;
    public float CellHeight => _cellHeight;
    public GridSize GridSize => gridSize;
    public Vector3[,] GridCells => _gridCells;
    public Vector3 CubeSize => _cubeSize;
    public GameObject[,] TilesAtGridCells => _tilesAtGridCells;

    private float _cellWidth;
    private float _cellHeight;
    private Vector3[,] _gridCells;
    private GameObject[,] _tilesAtGridCells;
    private Vector3 _cubeSize = new Vector3(0, 0, 0);

    private void OnEnable()
    {
        EditorApplication.playModeStateChanged += ResetDataOnPlayModeExit;
    }

    private void OnDisable()
    {
        EditorApplication.playModeStateChanged -= ResetDataOnPlayModeExit;
    }

    public void SetGridSize(GridSize newGridSize) => gridSize = newGridSize;

    public void AssignTileToCell(GameObject tile, Vector2Int cell) => _tilesAtGridCells[cell.x, cell.y] = tile;

    public Vector2Int GetTileGridCell(GameObject tileObject)
    {
        for (int i = 0; i < gridSize.Rows; i++)
        {
            for (int j = 0; j < gridSize.Columns; j++)
            {
                if (_tilesAtGridCells[i, j] == tileObject) return new Vector2Int(i, j);
            }
        }

        return new Vector2Int(-1, -1);
    }

    public bool AreTilesClose(Vector2Int firstGridCell, Vector2Int secondGridCell, out Axis closeInAxis)
    {
        bool areTilesClose = false;
        closeInAxis = Axis.None;

        if (firstGridCell.x == secondGridCell.x)
        {
            areTilesClose = Mathf.Abs(firstGridCell.y - secondGridCell.y) == 1;
            closeInAxis = Axis.Y;
        }
        else if (firstGridCell.y == secondGridCell.y)
        {
            areTilesClose = Mathf.Abs(firstGridCell.x - secondGridCell.x) == 1;
            closeInAxis = Axis.X;
        }

        return areTilesClose;
    }

    public void InitializeGrid(MeshRenderer gridRenderer)
    {
        int rows = gridSize.Rows;
        int columns = gridSize.Columns;

        _gridCells = new Vector3[rows, columns];
        _tilesAtGridCells = new GameObject[rows, columns];

        float gridOffset = gridRenderer.bounds.size.x * 0.05f;
        float gridWidth = gridRenderer.bounds.size.x - gridOffset;
        float gridHeight = gridRenderer.bounds.size.y - gridOffset;
        Vector3 gridCenter = gridRenderer.bounds.center;

        float cellWidth = gridWidth / rows;
        float cellHeight = gridHeight / columns;

        _cellWidth = cellWidth;
        _cellHeight = cellHeight;

        _cubeSize = new Vector3(cellWidth, cellHeight, 0.1f);

        for (int i = 0; i < rows; i++)
        {
            float xPosition = gridCenter.x - (gridWidth / 2) + (cellWidth / 2) + (i * cellWidth);

            for (int j = 0; j < columns; j++)
            {
                float yPosition = gridCenter.y + (gridHeight / 2) - (cellHeight / 2) - (j * cellHeight);

                _gridCells[i, j] = new Vector3(xPosition, yPosition, gridCenter.z);
            }
        }
    }

#if UNITY_EDITOR
    private void ResetDataOnPlayModeExit(PlayModeStateChange changedState)
    {
        if (changedState == PlayModeStateChange.ExitingPlayMode)
        {

        }
    }
#endif
}

public enum Axis
{
    None,
    X,
    Y
}
