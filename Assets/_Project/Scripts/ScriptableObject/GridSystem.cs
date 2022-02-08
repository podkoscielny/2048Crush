using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Crush2048
{
    [CreateAssetMenu(fileName = "GridSystem", menuName = "ScriptableObjects/GridSystem")]
    public class GridSystem : ScriptableObject
    {
        [SerializeField] GridSize gridSize;

        public GridSize GridSize => gridSize;
        public float CellWidth { get; private set; }
        public float CellHeight { get; private set; }
        public Vector3[,] GridCells { get; private set; }
        public GameObject[,] TilesAtGridCells { get; private set; }
        public TileType[,] TileTypeAtCell { get; private set; }
        public TileType[,] CachedTilesAtCells { get; private set; }
        public int[,] CachedPointsWorthAtCells { get; private set; }
        public int[,] PointsWorthAtCells { get; private set; }
        public Vector3 CubeSize { get; private set; } = new Vector3(0, 0, 0);

#if UNITY_EDITOR
        private void OnEnable() => EditorApplication.playModeStateChanged += ResetDataOnPlayModeExit;

        private void OnDisable() => EditorApplication.playModeStateChanged -= ResetDataOnPlayModeExit;
#endif

        public void SetGridSize(GridSize newGridSize) => gridSize = newGridSize;

        public void AssignTileToCell(GameObject tile, Vector2Int cell) => TilesAtGridCells[cell.x, cell.y] = tile;

        public void DeAssignTileFromCell(Vector2Int cell) => TilesAtGridCells[cell.x, cell.y] = null;

        public List<Vector2Int> GetEmptyTileCells()
        {
            List<Vector2Int> emptyCells = new List<Vector2Int>();

            for (int i = 0; i < gridSize.Rows; i++)
            {
                for (int j = 0; j < gridSize.Columns; j++)
                {
                    if (TilesAtGridCells[i, j] == null) emptyCells.Add(new Vector2Int(i, j));
                }
            }

            return emptyCells;
        }

        public Vector2Int GetTileGridCell(GameObject tileObject)
        {
            for (int i = 0; i < gridSize.Rows; i++)
            {
                for (int j = 0; j < gridSize.Columns; j++)
                {
                    if (TilesAtGridCells[i, j] == tileObject) return new Vector2Int(i, j);
                }
            }

            return new Vector2Int(-1, -1);
        }

        public bool AreTilesClose(Vector2Int firstGridCell, Vector2Int secondGridCell)
        {
            bool areTilesClose = false;

            if (firstGridCell.x == secondGridCell.x)
            {
                areTilesClose = Mathf.Abs(firstGridCell.y - secondGridCell.y) == 1;
            }
            else if (firstGridCell.y == secondGridCell.y)
            {
                areTilesClose = Mathf.Abs(firstGridCell.x - secondGridCell.x) == 1;
            }

            return areTilesClose;
        }

        public void AssignTileTypeAtCell(Vector2Int cell, TileType tileType) => TileTypeAtCell[cell.x, cell.y] = tileType;

        public void CacheTileTypeAtCell(Vector2Int cell, TileType tileType) => CachedTilesAtCells[cell.x, cell.y] = tileType;

        public void AssignPointsWorthAtCell(Vector2Int cell, int pointsWorth) => PointsWorthAtCells[cell.x, cell.y] = pointsWorth;

        public void CachePointsWorthAtCell(Vector2Int cell, int pointsWorth) => CachedPointsWorthAtCells[cell.x, cell.y] = pointsWorth;

        public List<Vector2Int> GetEmptyCellsInColumn(int columnIndex)
        {
            List<Vector2Int> emptyCellsInColumn = new List<Vector2Int>();

            for (int j = 0; j < GridSize.Rows; j++)
            {
                if (TilesAtGridCells[j, columnIndex] == null) emptyCellsInColumn.Add(new Vector2Int(j, columnIndex));
            }

            return emptyCellsInColumn;
        }

        public void ResetCellArrays()
        {
            int rows = gridSize.Rows;
            int columns = gridSize.Columns;

            TilesAtGridCells = new GameObject[rows, columns];
            CachedTilesAtCells = new TileType[rows, columns];
            TileTypeAtCell = new TileType[rows, columns];
            CachedPointsWorthAtCells = new int[rows, columns];
            PointsWorthAtCells = new int[rows, columns];
        }

        public void InitializeGrid(MeshRenderer gridRenderer)
        {
            int rows = gridSize.Rows;
            int columns = gridSize.Columns;

            InitializeCellArrays(rows, columns);

            float gridOffset = gridRenderer.bounds.size.x * 0.05f;
            float gridWidth = gridRenderer.bounds.size.x - gridOffset;
            float gridHeight = gridRenderer.bounds.size.y - gridOffset;
            Vector3 gridCenter = gridRenderer.bounds.center;

            CellWidth = gridWidth / rows;
            CellHeight = gridHeight / columns;

            CubeSize = new Vector3(CellWidth, CellHeight, 0.1f);

            for (int i = 0; i < rows; i++)
            {
                float yPosition = gridCenter.y + (gridHeight / 2) - (CellHeight / 2) - (i * CellHeight);

                for (int j = 0; j < columns; j++)
                {
                    float xPosition = gridCenter.x - (gridWidth / 2) + (CellWidth / 2) + (j * CellWidth);

                    GridCells[i, j] = new Vector3(xPosition, yPosition, gridCenter.z);
                }
            }
        }

        private void InitializeCellArrays(int rows, int columns)
        {
            GridCells = new Vector3[rows, columns];
            TilesAtGridCells = new GameObject[rows, columns];
            CachedTilesAtCells = new TileType[rows, columns];
            TileTypeAtCell = new TileType[rows, columns];
            CachedPointsWorthAtCells = new int[rows, columns];
            PointsWorthAtCells = new int[rows, columns];
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
}
