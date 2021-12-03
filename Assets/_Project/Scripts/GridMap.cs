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
    private SelectedTileProperties[] _selectedTiles;
    private Vector3 _cubeSize = new Vector3(0.5f, 0.5f, 0.5f);
    private Vector3 _tileRayDirection = new Vector3(0, 0, -1);

    private int _rows;
    private int _columns;

    private const float SWITCH_TILE_SPEED = .03f;
    private const int MINIMUM_TILES_TO_DESTROY = 3;

    void OnEnable()
    {
        Tile.OnTileClicked += HandleTileClick;
    }

    void OnDisable()
    {
        Tile.OnTileClicked -= HandleTileClick;
    }

    void Start()
    {
        _rows = gridSystem.GridSize.Rows;
        _columns = gridSystem.GridSize.Columns;

        _selectedTiles = new SelectedTileProperties[2];
        _gridCells = new Vector3[_rows, _columns];
        _tilesAtGridCells = new GameObject[_rows, _columns];
        InitializeGrid();
        AssignTilesToGridCells();
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

    private void HandleTileClick(SelectedTileProperties tileToBeSelected)
    {
        if (_selectedTiles[0].TileObject == null)
        {
            _selectedTiles[0] = tileToBeSelected;
        }
        else if (_selectedTiles[1].TileObject == null && _selectedTiles[0].TileObject != tileToBeSelected.TileObject)
        {
            Vector2Int firstTileSelectedCell = GetTileGridCell(_selectedTiles[0].TileObject);
            Vector2Int secondTileSelectedCell = GetTileGridCell(tileToBeSelected.TileObject);

            if (AreTilesClose(firstTileSelectedCell, secondTileSelectedCell))
            {
                _selectedTiles[1] = tileToBeSelected;
                StartCoroutine(SwitchTilesPosition());
            }
        }
    }

    private Vector2Int GetTileGridCell(GameObject tileObject)
    {
        for (int i = 0; i < _rows; i++)
        {
            for (int j = 0; j < _columns; j++)
            {
                if (_tilesAtGridCells[i, j] == tileObject) return new Vector2Int(i, j);
            }
        }

        return new Vector2Int(-1, -1);
    }

    private bool AreTilesClose(Vector2Int firstGridCell, Vector2Int secondGridCell)
    {
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

    private IEnumerator SwitchTilesPosition()
    {
        Rigidbody firstTileRb = _selectedTiles[0].TileRb;
        Rigidbody secondTileRb = _selectedTiles[1].TileRb;

        Vector3 initialFirstTilePosition = firstTileRb.position;
        Vector3 initialSecondTilePosition = secondTileRb.position;

        do
        {
            Vector3 firstTileDestination = Vector3.MoveTowards(firstTileRb.position, initialSecondTilePosition, 4f * Time.fixedDeltaTime);
            Vector3 secondTileDestination = Vector3.MoveTowards(secondTileRb.position, initialFirstTilePosition, 4f * Time.fixedDeltaTime);

            firstTileRb.MovePosition(firstTileDestination);
            secondTileRb.MovePosition(secondTileDestination);

            yield return new WaitForFixedUpdate();

        } while (firstTileRb.position != initialSecondTilePosition && secondTileRb.position != initialFirstTilePosition);

        AssignTilesToGridCells();
        DestroyTiles();
        ClearSelectedTiles();
    }

    private void DestroyTiles()
    {
        List<Vector2Int> tilesToBeDestroyed = CheckTilesToBeDestroyed(_selectedTiles[0].TileObject);
        List<Vector2Int> tilesToBeDestroyed2 = CheckTilesToBeDestroyed(_selectedTiles[1].TileObject);

        List<Vector2Int> tilesToDestroy = new List<Vector2Int>(tilesToBeDestroyed);

        foreach (Vector2Int tile in tilesToBeDestroyed2)
        {
            if (!tilesToDestroy.Contains(tile)) tilesToDestroy.Add(tile);
        }

        foreach (Vector2Int tile in tilesToDestroy)
        {
            Destroy(_tilesAtGridCells[tile.x, tile.y]);
        }
    }

    private void AssignTilesToGridCells()
    {
        for (int i = 0; i < _rows; i++)
        {
            for (int j = 0; j < _columns; j++)
            {
                Physics.Raycast(_gridCells[i, j], _tileRayDirection, out RaycastHit hitInfo, Mathf.Infinity, tileMask);

                GameObject tileHit = hitInfo.collider.gameObject;

                _tilesAtGridCells[i, j] = tileHit;
            }
        }
    }

    private List<Vector2Int> CheckTilesToBeDestroyed(GameObject selectedTile)
    {
        Vector2Int selectedTileCell = GetTileGridCell(selectedTile);

        List<Vector2Int> tilesToBeDestroyedInColumn = new List<Vector2Int>();
        List<Vector2Int> tilesToBeDestroyedInRow = new List<Vector2Int>();

        List<Vector2Int> tilesToDestroy = new List<Vector2Int>();

        for (int j = 0; j < _columns; j++)
        {
            if (_tilesAtGridCells[selectedTileCell.x, j].TryGetComponent(out Tile tile) && tile.TileType == _selectedTiles[0].TileType)
            {
                Vector2Int tileCell = new Vector2Int(selectedTileCell.x, j);
                tilesToBeDestroyedInColumn.Add(tileCell);
            }
            else
            {
                if (tilesToBeDestroyedInColumn.Count < MINIMUM_TILES_TO_DESTROY)
                {
                    tilesToBeDestroyedInColumn.Clear();
                }
                else
                {
                    bool isSelectedTileInColumn = false;

                    foreach (Vector2Int tileCell in tilesToBeDestroyedInColumn)
                    {
                        if (tileCell == selectedTileCell)
                        {
                            isSelectedTileInColumn = true;
                            break;
                        }
                    }

                    if (isSelectedTileInColumn)
                    {
                        break;
                    }
                    else
                    {
                        tilesToBeDestroyedInColumn.Clear();
                    }
                }
            }
        }

        if (tilesToBeDestroyedInColumn.Count < MINIMUM_TILES_TO_DESTROY) tilesToBeDestroyedInColumn.Clear();

        for (int j = 0; j < _rows; j++)
        {
            if (_tilesAtGridCells[j, selectedTileCell.y].TryGetComponent(out Tile tile) && tile.TileType == _selectedTiles[0].TileType)
            {
                Vector2Int tileCell = new Vector2Int(j, selectedTileCell.y);
                tilesToBeDestroyedInRow.Add(tileCell);
            }
            else
            {
                if (tilesToBeDestroyedInRow.Count < MINIMUM_TILES_TO_DESTROY)
                {
                    tilesToBeDestroyedInRow.Clear();
                }
                else
                {
                    bool isSelectedTileInRow = false;

                    foreach (Vector2Int tileCell in tilesToBeDestroyedInRow)
                    {
                        if (tileCell == selectedTileCell)
                        {
                            isSelectedTileInRow = true;
                            break;
                        }
                    }

                    if (isSelectedTileInRow)
                    {
                        break;
                    }
                    else
                    {
                        tilesToBeDestroyedInRow.Clear();
                    }
                }
            }
        }

        if (tilesToBeDestroyedInRow.Count < MINIMUM_TILES_TO_DESTROY) tilesToBeDestroyedInRow.Clear();

        tilesToDestroy.AddRange(tilesToBeDestroyedInColumn);

        foreach (Vector2Int tile in tilesToBeDestroyedInRow)
        {
            if (!tilesToDestroy.Contains(tile)) tilesToDestroy.Add(tile);
        }

        return tilesToDestroy;
    }

    private void ClearSelectedTiles() => _selectedTiles.Initialize();

    private void InitializeGrid()
    {
        float gridWidth = gridRenderer.bounds.size.x;
        float gridHeight = gridRenderer.bounds.size.y;
        Vector3 gridCenter = gridRenderer.bounds.center;

        float cellWidth = gridWidth / _rows;
        float cellHeight = gridHeight / _columns;

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