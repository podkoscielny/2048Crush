using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using Tags = TagSystem.Tags;

public class Board : MonoBehaviour
{
    public static event Action OnAssignPointsWorthToCells;
    public static event Action OnTileMatchEnded;
    public static event Action OnTilesReverse;
    public static event Action OnGameOver;

    [SerializeField] Camera mainCamera;
    [SerializeField] GridSystem gridSystem;
    [SerializeField] MeshRenderer gridRenderer;
    [SerializeField] GameObject tilePrefab;
    [SerializeField] ObjectPool objectPool;
    [SerializeField] Score score;

    public static bool CanTilesBeClicked { get; private set; } = true;

    private Sequence _tileMoveSequence;
    private Vector3 _enlargedTileScale = new Vector3(0.4f, 0.4f, 0.4f);

    private void OnEnable()
    {
        TileSwipe.OnTilesMatch += MatchTiles;
        SpecialTile.OnSpecialTileUsed += MatchWithSpecialTile;
    }
    private void OnDisable()
    {
        TileSwipe.OnTilesMatch -= MatchTiles;
        SpecialTile.OnSpecialTileUsed -= MatchWithSpecialTile;
    }

    private void Awake()
    {
        InitializeBoardSize();
        gridSystem.InitializeGrid(gridRenderer);
        objectPool.InitializePool();
    }

    private void Start() => InitializeTiles();

    public void ReverseMove()
    {
        if (!CanTilesBeClicked || gridSystem.CachedPointsWorthAtCells == null) return;

        OnTilesReverse?.Invoke();
    }

    private void MatchTiles(SelectedTile tileToBeDestroyed, Transform tileToBeUpdated, TilePoints updatedTilePoints)
    {
        CanTilesBeClicked = false;
        OnAssignPointsWorthToCells?.Invoke();
        gridSystem.CachePreviousPoints();

        _tileMoveSequence = DOTween.Sequence().SetAutoKill(false);
        _tileMoveSequence.Append(tileToBeDestroyed.TileObject.transform.DODynamicLookAt(tileToBeUpdated.position, 0.1f));
        _tileMoveSequence.Append(tileToBeDestroyed.TileObject.transform.DOMove(tileToBeUpdated.position, 0.15f).SetEase(Ease.InBack));
        _tileMoveSequence.AppendCallback(() => MoveTileToPool(tileToBeDestroyed.TileObject));
        _tileMoveSequence.AppendCallback(() => UpdateScore(2, updatedTilePoints));
        _tileMoveSequence.AppendCallback(() => SpawnMissingTile(tileToBeDestroyed.TileCell));
        _tileMoveSequence.AppendCallback(CheckPossibleMoves);
        _tileMoveSequence.AppendCallback(() => OnTileMatchEnded?.Invoke());
        _tileMoveSequence.Append(tileToBeUpdated.DOPunchScale(_enlargedTileScale, 0.3f, 1));
    }

    private void MatchWithSpecialTile(SelectedTile selectedTile, SpecialTileBehaviour tileBehaviour, GameObject specialTile)
    {
        CanTilesBeClicked = false;
        Transform specialTileTransform = specialTile.transform;

        _tileMoveSequence = DOTween.Sequence().SetAutoKill(false);
        _tileMoveSequence.Append(selectedTile.TileObject.transform.DODynamicLookAt(specialTileTransform.position, 0.1f));
        _tileMoveSequence.Append(selectedTile.TileObject.transform.DOMove(specialTileTransform.position, 0.15f).SetEase(Ease.InBack));
        _tileMoveSequence.AppendCallback(() => tileBehaviour(selectedTile, specialTile));
        _tileMoveSequence.AppendCallback(() => SpawnMissingTile(selectedTile.TileCell));
        _tileMoveSequence.AppendCallback(CheckPossibleMoves);
    }

    private void UpdateScore(int pointsMultiplier, TilePoints tilePoints)
    {
        tilePoints.MultiplyPoints(pointsMultiplier);
        score.AddPoints(tilePoints.PointsWorth);
    }

    private void MoveTileToPool(GameObject tileToBeDisabled) => objectPool.AddToPool(Tags.Tile, tileToBeDisabled);

    private void SpawnMissingTile(Vector2Int disabledTileGridCell)
    {
        Vector2Int firstCellInColumn = new Vector2Int(0, disabledTileGridCell.y);
        Vector3 firstGridCellPosition = gridSystem.GridCells[0, disabledTileGridCell.y];
        Vector3 spawnPosition = new Vector3(firstGridCellPosition.x, firstGridCellPosition.y + (gridSystem.CellHeight * 1.4f), tilePrefab.transform.position.z);

        GameObject spawnedTile = objectPool.GetFromPool(Tags.Tile);
        //GameObject spawnedTile = objectPool.GetFromPool(Tags.SpecialTile);
        spawnedTile.transform.position = spawnPosition;
        spawnedTile.transform.DOMoveY(firstGridCellPosition.y, 0.2f).SetDelay(0.1f);

        MoveTilesDown(disabledTileGridCell);

        gridSystem.AssignTileToCell(spawnedTile, firstCellInColumn);
    }

    private void MoveTilesDown(Vector2Int disabledTileGridCell)
    {
        int columnIndex = disabledTileGridCell.y;

        if (disabledTileGridCell.x == 0) return;

        for (int i = disabledTileGridCell.x - 1; i >= 0; i--)
        {
            Vector2Int nextGridCell = new Vector2Int(i + 1, columnIndex);
            float desiredPositionY = gridSystem.GridCells[i + 1, columnIndex].y;

            GameObject tileToBeMoved = gridSystem.TilesAtGridCells[i, columnIndex];
            tileToBeMoved.transform.DOMoveY(desiredPositionY, 0.2f);

            gridSystem.AssignTileToCell(tileToBeMoved, nextGridCell);
        }
    }

    private void CheckPossibleMoves()
    {
        OnAssignPointsWorthToCells?.Invoke();

        int rows = gridSystem.GridSize.Rows;
        int columns = gridSystem.GridSize.Columns;

        for (int i = 0; i < rows; i++)
        {
            for (int j = 0; j < columns; j++)
            {
                int pointsWorthAtCell = gridSystem.PointsWorthAtGridCells[i, j];

                bool canBeMargedInColumn = i < rows - 1 && gridSystem.PointsWorthAtGridCells[i + 1, j] == pointsWorthAtCell;
                bool canBeMargedInRow = j < columns - 1 && gridSystem.PointsWorthAtGridCells[i, j + 1] == pointsWorthAtCell;

                if (canBeMargedInColumn || canBeMargedInRow)
                {
                    CanTilesBeClicked = true;
                    return;
                }
            }
        }

        EndGame();
    }

    private void EndGame()
    {
        CanTilesBeClicked = false;
        OnGameOver?.Invoke();
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
