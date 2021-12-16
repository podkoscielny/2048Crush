using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Tags = TagSystem.Tags;

public class GridMap : MonoBehaviour
{
    [SerializeField] Camera mainCamera;
    [SerializeField] GridSystem gridSystem;
    [SerializeField] MeshRenderer gridRenderer;
    [SerializeField] GameObject tilePrefab;
    [SerializeField] ObjectPool objectPool;
    [SerializeField] Score score;

    private SelectedTileProperties[] _selectedTiles = new SelectedTileProperties[2];
    private SelectedTileProperties _emptyProperties = new SelectedTileProperties();

    private int _rows;
    private int _columns;
    private RigidbodyConstraints _fullTileConstraints;
    private RigidbodyConstraints _unlockedTileConstraints;
    private RigidbodyConstraints _semiUnlockedTileConstraints;

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

    private void Awake()
    {
        _fullTileConstraints = RigidbodyConstraints.FreezeAll;
        _unlockedTileConstraints = RigidbodyConstraints.FreezeRotation | RigidbodyConstraints.FreezePositionZ;
        _semiUnlockedTileConstraints = RigidbodyConstraints.FreezeRotation | RigidbodyConstraints.FreezePositionZ | RigidbodyConstraints.FreezePositionX;

        InitializeBoardSize();
        gridSystem.InitializeGrid(gridRenderer);
        objectPool.InitializePool();
    }

    private void Start()
    {
        _rows = gridSystem.GridSize.Rows;
        _columns = gridSystem.GridSize.Columns;

        InitializeTiles();
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
                _selectedTiles[1] = tileToBeSelected;
                StartCoroutine(MatchTiles());
            }
            else
            {
                ClearSelectedTiles();
            }
        }
    }

    private IEnumerator MatchTiles()
    {
        yield return StartCoroutine(SwitchTilesPosition());

        gridSystem.AssignTilesToGridCells();
        List<Vector2Int> tilesToDestroy = DestroyTiles();

        if (tilesToDestroy.Count > 0)
        {
            SpawnMissingTiles();
            yield return StartCoroutine(AddGravityToTiles());
            gridSystem.AssignTilesToGridCells();
            ClearSelectedTiles();
        }
        else
        {
            yield return new WaitForSeconds(0.1f);
            yield return SwitchTilesPosition();
            gridSystem.AssignTilesToGridCells();
            ClearSelectedTiles();
        }
    }

    private IEnumerator SwitchTilesPosition()
    {
        Rigidbody firstTileRb = _selectedTiles[0].TileRb;
        Rigidbody secondTileRb = _selectedTiles[1].TileRb;

        Vector3 initialFirstTilePosition = firstTileRb.position;
        Vector3 initialSecondTilePosition = secondTileRb.position;

        firstTileRb.constraints = _unlockedTileConstraints;
        secondTileRb.constraints = _unlockedTileConstraints;

        firstTileRb.isKinematic = true;
        secondTileRb.isKinematic = true;

        do
        {
            Vector3 firstTileDestination = Vector3.MoveTowards(firstTileRb.position, initialSecondTilePosition, SWITCH_TILE_SPEED * Time.fixedDeltaTime);
            Vector3 secondTileDestination = Vector3.MoveTowards(secondTileRb.position, initialFirstTilePosition, SWITCH_TILE_SPEED * Time.fixedDeltaTime);

            firstTileRb.MovePosition(firstTileDestination);
            secondTileRb.MovePosition(secondTileDestination);

            yield return new WaitForFixedUpdate();

        } while (firstTileRb.position != initialSecondTilePosition && secondTileRb.position != initialFirstTilePosition);

        firstTileRb.constraints = _fullTileConstraints;
        secondTileRb.constraints = _fullTileConstraints;

        firstTileRb.isKinematic = false;
        secondTileRb.isKinematic = false;
    }

    private List<Vector2Int> DestroyTiles()
    {
        List<Vector2Int> tilesToBeDestroyed = CheckTilesToBeDestroyed(_selectedTiles[0]);
        List<Vector2Int> tilesToBeDestroyed2 = CheckTilesToBeDestroyed(_selectedTiles[1]);

        List<Vector2Int> tilesToDestroy = new List<Vector2Int>(tilesToBeDestroyed);

        foreach (Vector2Int tile in tilesToBeDestroyed2)
        {
            if (!tilesToDestroy.Contains(tile)) tilesToDestroy.Add(tile);
        }

        int pointsToAdd = 0;

        foreach (Vector2Int tile in tilesToDestroy)
        {
            objectPool.AddToPool(Tags.Tile, gridSystem.TilesAtGridCells[tile.x, tile.y]);

            if (gridSystem.TilesAtGridCells[tile.x, tile.y].TryGetComponent(out Tile tileScript))
            {
                pointsToAdd += tileScript.TileType.PointsWorth;
            }
        }

        score.AddPoints(pointsToAdd);

        return tilesToDestroy;
    }

    private void SpawnMissingTiles()
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

            float tileY = gridSystem.GridCells[0, 0].y + (gridSystem.CellHeight * 1.5f) + (missingInColumn * (gridRenderer.bounds.size.y / _columns + 0.05f));
            Vector3 tilePosition = new Vector3(cellPosition.x, tileY, tilePrefab.transform.position.z);

            GameObject tile = objectPool.GetFromPool(Tags.Tile);
            tile.transform.SetPositionAndRotation(tilePosition, tilePrefab.transform.rotation);
        }
    }

    private IEnumerator AddGravityToTiles()
    {
        var activeTiles = TagSystem.FindAllGameObjectsWithTag(Tags.Tile);
        List<Rigidbody> tileRigidBodys = new List<Rigidbody>();

        foreach (var tile in activeTiles)
        {
            if (tile.TryGetComponent(out Rigidbody tileRb))
            {
                tileRb.constraints = _semiUnlockedTileConstraints;
                tileRigidBodys.Add(tileRb);
            }
        }

        bool areTilesMoving = true;

        yield return new WaitForSeconds(0.1f);

        do
        {
            int notMovingTilesCount = 0;

            foreach (Rigidbody tileRb in tileRigidBodys)
            {
                if (tileRb.velocity.y == 0) notMovingTilesCount++;
            }

            if (notMovingTilesCount == tileRigidBodys.Count) areTilesMoving = false;

            yield return null;

        } while (areTilesMoving);
    }

    private List<Vector2Int> CheckTilesToBeDestroyed(SelectedTileProperties selectedTile)
    {
        Vector2Int selectedTileCell = gridSystem.GetTileGridCell(selectedTile.TileObject);

        List<Vector2Int> tilesToBeDestroyedInColumn = new List<Vector2Int>();
        List<Vector2Int> tilesToBeDestroyedInRow = new List<Vector2Int>();

        List<Vector2Int> tilesToDestroy = new List<Vector2Int>();

        for (int j = 0; j < _columns; j++)
        {
            if (gridSystem.TilesAtGridCells[selectedTileCell.x, j].TryGetComponent(out Tile tile) && tile.TileType == selectedTile.TileType)
            {
                Vector2Int tileCell = new Vector2Int(selectedTileCell.x, j);
                tilesToBeDestroyedInColumn.Add(tileCell);
            }
            else
            {
                bool isSelectedTileInColumn = tilesToBeDestroyedInColumn.Contains(selectedTileCell);

                if (tilesToBeDestroyedInColumn.Count < MINIMUM_TILES_TO_DESTROY || !isSelectedTileInColumn)
                {
                    tilesToBeDestroyedInColumn.Clear();
                }
                else
                {
                    if (isSelectedTileInColumn) break;
                }
            }
        }

        if (tilesToBeDestroyedInColumn.Count < MINIMUM_TILES_TO_DESTROY) tilesToBeDestroyedInColumn.Clear();

        for (int j = 0; j < _rows; j++)
        {
            if (gridSystem.TilesAtGridCells[j, selectedTileCell.y].TryGetComponent(out Tile tile) && tile.TileType == selectedTile.TileType)
            {
                Vector2Int tileCell = new Vector2Int(j, selectedTileCell.y);
                tilesToBeDestroyedInRow.Add(tileCell);
            }
            else
            {
                bool isSelectedTileInRow = tilesToBeDestroyedInRow.Contains(selectedTileCell);

                if (tilesToBeDestroyedInRow.Count < MINIMUM_TILES_TO_DESTROY || !isSelectedTileInRow)
                {
                    tilesToBeDestroyedInRow.Clear();
                }
                else
                {
                    if (isSelectedTileInRow) break;
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

    private void InitializeTiles()
    {
        Vector3[,] gridCells = gridSystem.GridCells;

        for (int i = 0; i < gridCells.GetLength(0); i++)
        {
            for (int j = 0; j < gridCells.GetLength(1); j++)
            {
                GameObject tile = objectPool.GetFromPool(Tags.Tile);
                Vector3 tilePosition = new Vector3(gridCells[i, j].x, gridCells[i, j].y, tilePrefab.transform.position.z);

                tile.transform.SetPositionAndRotation(tilePosition, tilePrefab.transform.rotation);
                gridSystem.AssignTileToCell(tile, new Vector2Int(i, j));
            }
        }
    }

    private void InitializeBoardSize()
    {
        int screenWidth = Screen.width;
        int screenHeight = Screen.height;

        float aspectRatio = (float)screenWidth / (float)screenHeight;

        transform.localScale = (Vector3.one * 0.95f) * aspectRatio;

        float yPosition = mainCamera.transform.position.y - (aspectRatio + transform.position.y);
        Vector3 boardPosition = new Vector3(transform.position.x, yPosition, transform.position.z);
        transform.position = boardPosition;
    }
}