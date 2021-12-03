using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridMap : MonoBehaviour
{
    [SerializeField] GridSystem gridSystem;
    [SerializeField] MeshRenderer gridRenderer;
    [SerializeField] LayerMask tileMask;

    private Vector3[,] _gridCells;
    private GameObject[,] _tilesAtGridCells;
    private Vector3 _cubeSize = new Vector3(0.5f, 0.5f, 0.5f);
    private Vector3 _tileRayDirection = new Vector3(0, 0, -1);
    private Vector3 _firstTileVelocityRef = Vector3.zero;
    private Vector3 _secondTileVelocityRef = Vector3.zero;

    private int _rows;
    private int _columns;
    private float _cellWidth;
    private float _cellHeight;

    private const float SWITCH_TILE_SPEED = .03f;
    private const int MINIMUM_TILES_TO_DESTROY = 3;

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

        _gridCells = new Vector3[_rows, _columns];
        _tilesAtGridCells = new GameObject[_rows, _columns];
        InitializeGrid();
        //Raycast cache tiles on grid
    }

    void OnDrawGizmos()
    {
        if (_gridCells != null)
        {
            for (int i = 0; i < _gridCells.GetLength(0); i++)
            {
                for (int j = 0; j < _gridCells.GetLength(1); j++)
                {
                    Gizmos.DrawWireCube(_gridCells[i, j], _cubeSize);
                }
            }
        }
    }

    private void HandleSwitchTiles(List<SelectedTileProperties> tilesToSwitch)
    {
        List<SelectedTileProperties> selectedTiles = new List<SelectedTileProperties>(tilesToSwitch);

        Vector2Int[] selectedTilesGridCells = GetSelectedTilesGridCells(selectedTiles);

        bool areTilesClose = AreTilesClose(selectedTilesGridCells);

        if (!areTilesClose) return;

        StartCoroutine(SwitchTilesPosition(selectedTiles, selectedTilesGridCells));
    }

    private Vector2Int[] GetSelectedTilesGridCells(List<SelectedTileProperties> selectedTiles)
    {
        Vector2Int firstTileGridCell = new Vector2Int(-1, -1);
        Vector2Int secondTileGridCell = new Vector2Int(-1, -1);


        for (int i = 0; i < _rows; i++)
        {
            for (int j = 0; j < _columns; j++)
            {
                Physics.Raycast(_gridCells[i, j], _tileRayDirection, out RaycastHit hitInfo, Mathf.Infinity, tileMask);

                GameObject tileHit = hitInfo.collider.gameObject;

                if (selectedTiles[0].TileObject == tileHit)
                {
                    firstTileGridCell.x = i;
                    firstTileGridCell.y = j;
                }
                else if (selectedTiles[1].TileObject == tileHit)
                {
                    secondTileGridCell.x = i;
                    secondTileGridCell.y = j;
                }

                _tilesAtGridCells[i, j] = tileHit;
            }
        }

        Vector2Int[] selectedTilesGridCells = new Vector2Int[2] { firstTileGridCell, secondTileGridCell };

        return selectedTilesGridCells;
    }

    private bool AreTilesClose(Vector2Int[] selectedTilesGridCells)
    {
        Vector2Int firstGridCell = selectedTilesGridCells[0];
        Vector2Int secondGridCell = selectedTilesGridCells[1];

        if (firstGridCell.x == secondGridCell.x)
        {
            return Mathf.Abs(firstGridCell.y - secondGridCell.y) == 1;
        }
        else if (firstGridCell.y == secondGridCell.y)
        {
            return Mathf.Abs(firstGridCell.x - secondGridCell.x) == 1;
        }

        return false;
    }

    private IEnumerator SwitchTilesPosition(List<SelectedTileProperties> selectedTiles, Vector2Int[] selectedTilesGridCells)
    {
        Rigidbody firstTileRb = selectedTiles[0].TileRb;
        Rigidbody secondTileRb = selectedTiles[1].TileRb;

        Vector3 initialFirstTilePosition = firstTileRb.position;
        Vector3 initialSecondTilePosition = secondTileRb.position;

        Vector2Int initialFirstTileGridCell = selectedTilesGridCells[0];
        Vector2Int initialSecondTileGridCell = selectedTilesGridCells[1];

        do
        {
            //Vector3 firstTileDestination = Vector3.SmoothDamp(firstTileRb.position, initialSecondTilePosition, ref _firstTileVelocityRef, SWITCH_TILE_SPEED);
            //Vector3 secondTileDestination = Vector3.SmoothDamp(secondTileRb.position, initialFirstTilePosition, ref _secondTileVelocityRef, SWITCH_TILE_SPEED);
            Vector3 firstTileDestination = Vector3.MoveTowards(firstTileRb.position, initialSecondTilePosition, 8f * Time.deltaTime);
            Vector3 secondTileDestination = Vector3.MoveTowards(secondTileRb.position, initialFirstTilePosition, 8f * Time.deltaTime);

            firstTileRb.MovePosition(firstTileDestination);
            secondTileRb.MovePosition(secondTileDestination);

            yield return null;

        } while (firstTileRb.position != initialSecondTilePosition && secondTileRb.position != initialFirstTilePosition);

        selectedTilesGridCells[0] = initialSecondTileGridCell;
        selectedTilesGridCells[1] = initialFirstTileGridCell;

        CheckTilesToBeDestroyed(selectedTilesGridCells, selectedTiles);
    }

    private void CheckTilesToBeDestroyed(Vector2Int[] selectedTilesGridCells, List<SelectedTileProperties> selectedTiles)
    {
        List<Vector2Int> sameTilesAsFirstInColumn = GetSameTileTypesInColumn(selectedTiles[0].TileType, selectedTilesGridCells[0].y);
        List<Vector2Int> sameTilesAsFirstInRow = GetSameTileTypesInRow(selectedTiles[0].TileType, selectedTilesGridCells[0].x);
        List<Vector2Int> sameTilesAsSecondInColumn = GetSameTileTypesInColumn(selectedTiles[1].TileType, selectedTilesGridCells[1].y);
        List<Vector2Int> sameTilesAsSecondInRow = GetSameTileTypesInRow(selectedTiles[1].TileType, selectedTilesGridCells[1].x);

        Debug.Log($"Same first Column: {sameTilesAsFirstInColumn.Count}, Same first Row: {sameTilesAsFirstInRow.Count}, Same second Column: {sameTilesAsSecondInColumn.Count}, Same second Row: {sameTilesAsSecondInRow.Count}");
    }

    private List<Vector2Int> GetSameTileTypesInRow(TileType selectedTileType, int rowIndex)
    {
        List<Vector2Int> sameAsTile = new List<Vector2Int>();

        for (int i = 0; i < _columns; i++)
        {
            _tilesAtGridCells[rowIndex, i].TryGetComponent(out Tile tile);

            if (tile.TileType == selectedTileType)
            {
                sameAsTile.Add(new Vector2Int(rowIndex, i));
            }
            //else
            //{
            //    if (sameAsTile.Count < MINIMUM_TILES_TO_DESTROY)
            //    {
            //        sameAsTile.Clear();
            //    }
            //    else
            //    {
            //        break;
            //    }
            //}
        }

        return sameAsTile;
    }

    private List<Vector2Int> GetSameTileTypesInColumn(TileType selectedTileType, int columnIndex)
    {
        List<Vector2Int> sameAsTile = new List<Vector2Int>();

        for (int i = 0; i < _rows; i++)
        {
            _tilesAtGridCells[i, columnIndex].TryGetComponent(out Tile tile);

            if (tile.TileType == selectedTileType)
            {
                sameAsTile.Add(new Vector2Int(i, columnIndex));
            }
            //else
            //{
            //    if (sameAsTile.Count < MINIMUM_TILES_TO_DESTROY)
            //    {
            //        sameAsTile.Clear();
            //    }
            //    else
            //    {
            //        break;
            //    }
            //}
        }

        return sameAsTile;
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
                float yPosition = gridCenter.y + (gridHeight / 2) - (cellHeight / 2) - (j * cellHeight);

                _gridCells[i, j] = new Vector3(xPosition, yPosition, gridCenter.z);
            }
        }
    }
}