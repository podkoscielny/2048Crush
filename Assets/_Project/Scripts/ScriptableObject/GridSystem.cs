using System;
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
        public int[,] PointsWorthAtGridCells { get; private set; }
        public int[,] CachedPointsWorthAtCells { get; private set; }
        public Vector3 CubeSize { get; private set; } = new Vector3(0, 0, 0);

        private void OnEnable() => EditorApplication.playModeStateChanged += ResetDataOnPlayModeExit;

        private void OnDisable() => EditorApplication.playModeStateChanged -= ResetDataOnPlayModeExit;

        public void SetGridSize(GridSize newGridSize) => gridSize = newGridSize;

        public void AssignTileToCell(GameObject tile, Vector2Int cell) => TilesAtGridCells[cell.x, cell.y] = tile;

        public void DeAssignTileFromCell(Vector2Int cell) => TilesAtGridCells[cell.x, cell.y] = null;

        public Vector2Int GetEmptyTileCell()
        {
            Vector2Int emptyCell = new Vector2Int(-1, -1);

            for (int i = 0; i < gridSize.Rows; i++)
            {
                for (int j = 0; j < gridSize.Columns; j++)
                {
                    if (TilesAtGridCells[i, j] == null) return new Vector2Int(i, j);
                }
            }

            return emptyCell;
        }

        public void AssignPointsWorthToCell(int pointsWorth, Vector2Int cell) => PointsWorthAtGridCells[cell.x, cell.y] = pointsWorth;

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

        public void CachePreviousPoints()
        {
            CachedPointsWorthAtCells = new int[gridSize.Rows, gridSize.Columns];

            Array.Copy(PointsWorthAtGridCells, CachedPointsWorthAtCells, PointsWorthAtGridCells.Length);
        }

        public void InitializeGrid(MeshRenderer gridRenderer)
        {
            int rows = gridSize.Rows;
            int columns = gridSize.Columns;

            GridCells = new Vector3[rows, columns];
            TilesAtGridCells = new GameObject[rows, columns];
            PointsWorthAtGridCells = new int[rows, columns];

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
}
