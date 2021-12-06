using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridMap : MonoBehaviour
{
    [SerializeField] GridSystem gridSystem;
    [SerializeField] MeshRenderer gridRenderer;
    [SerializeField] LayerMask tileMask;
    [SerializeField] GameObject tilePrefab;
    [SerializeField] Transform spawnPosition;

    private SelectedTileProperties[] _selectedTiles;
    private Vector3 _tileRayDirection = new Vector3(0, 0, -1);
    private SelectedTileProperties _emptyProperties = new SelectedTileProperties();

    private int _rows;
    private int _columns;

    private const float SPAWN_TILE_Y_POSITION = 0.9f;
    private const float SWITCH_TILE_SPEED = 4f;
    private const int MINIMUM_TILES_TO_DESTROY = 3;

    private void OnEnable()
    {
        Tile.OnTileClicked += HandleTileClick;
    }

    private void OnDisable()
    {
        Tile.OnTileClicked -= HandleTileClick;
    }

    private void Start()
    {
        _rows = gridSystem.GridSize.Rows;
        _columns = gridSystem.GridSize.Columns;

        _selectedTiles = new SelectedTileProperties[2];
        gridSystem.InitializeGrid(gridRenderer);
        gridSystem.AssignTilesToGridCells();
    }

    void OnDrawGizmos()
    {
        if (gridSystem.GridCells != null)
        {
            for (int i = 0; i < gridSystem.GridCells.GetLength(0); i++)
            {
                for (int j = 0; j < gridSystem.GridCells.GetLength(1); j++)
                {
                    Gizmos.DrawWireCube(gridSystem.GridCells[i, j], gridSystem.CubeSize);
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
            Vector2Int firstTileSelectedCell = gridSystem.GetTileGridCell(_selectedTiles[0].TileObject);
            Vector2Int secondTileSelectedCell = gridSystem.GetTileGridCell(tileToBeSelected.TileObject);

            if (gridSystem.AreTilesClose(firstTileSelectedCell, secondTileSelectedCell))
            {
                _selectedTiles[1] = tileToBeSelected; // Add Coroutine Queue method
                StartCoroutine(SwitchTilesPosition());
            }
        }
    }

    private IEnumerator SwitchTilesPosition()
    {
        Rigidbody firstTileRb = _selectedTiles[0].TileRb;
        Rigidbody secondTileRb = _selectedTiles[1].TileRb;

        Vector3 initialFirstTilePosition = firstTileRb.position;
        Vector3 initialSecondTilePosition = secondTileRb.position;

        do
        {
            Vector3 firstTileDestination = Vector3.MoveTowards(firstTileRb.position, initialSecondTilePosition, SWITCH_TILE_SPEED * Time.fixedDeltaTime);
            Vector3 secondTileDestination = Vector3.MoveTowards(secondTileRb.position, initialFirstTilePosition, SWITCH_TILE_SPEED * Time.fixedDeltaTime);

            firstTileRb.MovePosition(firstTileDestination);
            secondTileRb.MovePosition(secondTileDestination);

            yield return new WaitForFixedUpdate();

        } while (firstTileRb.position != initialSecondTilePosition && secondTileRb.position != initialFirstTilePosition);

        gridSystem.AssignTilesToGridCells();
        StartCoroutine(DestroyTiles());
    }

    private IEnumerator DestroyTiles()
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
            Destroy(gridSystem.TilesAtGridCells[tile.x, tile.y]);
        }

        yield return new WaitForEndOfFrame();

        if (tilesToDestroy.Count > 0) StartCoroutine(SpawnMissingTiles());
    }

    private IEnumerator SpawnMissingTiles()
    {
        List<Vector2Int> emptyCells = gridSystem.GetEmptyGridCells();
        int columnIndex = -1;
        int missingInColumn = 0;

        foreach (Vector2Int cell in emptyCells)
        {
            Vector3 cellPosition = gridSystem.GetCellCoordinates(cell.x, cell.y);

            if (cell.x == columnIndex)
            {
                missingInColumn++;
            }
            else
            {
                missingInColumn = 0;
                columnIndex = cell.x;
            }

            float tileY = spawnPosition.position.y + (missingInColumn * (gridRenderer.bounds.size.y / gridSystem.GridSize.Columns + 0.05f));
            Vector3 tilePosition = new Vector3(cellPosition.x, tileY, spawnPosition.position.z);

            Instantiate(tilePrefab, tilePosition, tilePrefab.transform.rotation);
        }

        yield return new WaitForEndOfFrame();
    }

    private List<Vector2Int> CheckTilesToBeDestroyed(GameObject selectedTile)
    {
        Vector2Int selectedTileCell = gridSystem.GetTileGridCell(selectedTile);

        List<Vector2Int> tilesToBeDestroyedInColumn = new List<Vector2Int>();
        List<Vector2Int> tilesToBeDestroyedInRow = new List<Vector2Int>();

        List<Vector2Int> tilesToDestroy = new List<Vector2Int>();

        for (int j = 0; j < _columns; j++)
        {
            if (gridSystem.TilesAtGridCells[selectedTileCell.x, j].TryGetComponent(out Tile tile) && tile.TileType == _selectedTiles[0].TileType)
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
            if (gridSystem.TilesAtGridCells[j, selectedTileCell.y].TryGetComponent(out Tile tile) && tile.TileType == _selectedTiles[0].TileType)
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

    private void ClearSelectedTiles()
    {
        _selectedTiles[0] = _emptyProperties;
        _selectedTiles[1] = _emptyProperties;
    }
}