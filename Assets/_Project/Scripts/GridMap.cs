using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridMap : MonoBehaviour
{
    [SerializeField] GridSystem gridSystem;
    [SerializeField] MeshRenderer gridRenderer;
    [SerializeField] LayerMask tileMask;

    private Vector3[,] gridCells;
    private Vector3 _cubeSize = new Vector3(0.5f, 0.5f, 0.5f);
    private Vector3 _tileRayDirection = new Vector3(0, 0, -1);
    private Vector3 _firstTileVelocityRef = Vector3.zero;
    private Vector3 _secondTileVelocityRef = Vector3.zero;

    private int _rows;
    private int _columns;
    private float _cellWidth;
    private float _cellHeight;

    private const float SWITCH_TILE_SPEED = .03f;

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
        _rows = gridSystem.GridSize.Rows;
        _columns = gridSystem.GridSize.Columns;

        gridCells = new Vector3[_rows, _columns];
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
        bool areTilesClose = AreTilesClose(selectedTiles);

        if (!areTilesClose) return;

        StartCoroutine(SwitchTilesPosition(selectedTiles));
    }

    private IEnumerator SwitchTilesPosition(List<SelectedTileProperties> selectedTiles)
    {
        Rigidbody firstTileRb = selectedTiles[0].TileRb;
        Rigidbody secondTileRb = selectedTiles[1].TileRb;

        Vector3 initialFirstTilePosition = firstTileRb.position;
        Vector3 initialSecondTilePosition = secondTileRb.position;

        do
        {
            Vector3 firstTileDestination = Vector3.SmoothDamp(firstTileRb.position, initialSecondTilePosition, ref _firstTileVelocityRef, SWITCH_TILE_SPEED);
            Vector3 secondTileDestination = Vector3.SmoothDamp(secondTileRb.position, initialFirstTilePosition, ref _secondTileVelocityRef, SWITCH_TILE_SPEED);

            firstTileRb.MovePosition(firstTileDestination);
            secondTileRb.MovePosition(secondTileDestination);

            yield return null;

        } while (firstTileRb.position != initialSecondTilePosition && secondTileRb.position != initialFirstTilePosition);
    }

    private bool AreTilesClose(List<SelectedTileProperties> selectedTiles)
    {
        int firstTileRow = -1;
        int firstTileColumn = -1;

        int secondTileRow = -1;
        int secondTileColumn = -1;

        for (int i = 0; i < _rows; i++)
        {
            for (int j = 0; j < _columns; j++)
            {
                Physics.Raycast(gridCells[i, j], _tileRayDirection, out RaycastHit hitInfo, Mathf.Infinity, tileMask);

                GameObject tileHit = hitInfo.collider.gameObject;

                if (selectedTiles[0].TileObject == tileHit)
                {
                    firstTileRow = i;
                    firstTileColumn = j;
                }
                else if (selectedTiles[1].TileObject == tileHit)
                {
                    secondTileRow = i;
                    secondTileColumn = j;
                }
            }
        }

        if (firstTileRow == -1 || secondTileRow == -1)
        {
            return false;
        }
        else if (firstTileRow == secondTileRow)
        {
            return Mathf.Abs(firstTileColumn - secondTileColumn) == 1;
        }
        else if (firstTileColumn == secondTileColumn)
        {
            return Mathf.Abs(firstTileRow - secondTileRow) == 1;
        }

        return false;
    }

    private void InitializeGrid()
    {
        float gridWidth = gridRenderer.bounds.size.x;
        float gridHeight = gridRenderer.bounds.size.y;
        Vector3 gridCenter = gridRenderer.bounds.center;

        float cellWidth = gridWidth / _rows;
        float cellHeight = gridHeight / _columns;

        _cellWidth = cellWidth;
        _cellHeight = cellHeight;

        _cubeSize = new Vector3(cellWidth, cellHeight, 0.1f);

        for (int i = 0; i < _rows; i++)
        {
            float xPosition = gridCenter.x - (gridWidth / 2) + (cellWidth / 2) + (i * cellWidth);

            for (int j = 0; j < _columns; j++)
            {
                float yPosition = gridCenter.y - (gridHeight / 2) + (cellHeight / 2) + (j * cellHeight);

                gridCells[i, j] = new Vector3(xPosition, yPosition, gridCenter.z);
            }
        }
    }
}