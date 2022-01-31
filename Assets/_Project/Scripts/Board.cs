using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using Tags = TagSystem.TagSystem.Tags;

namespace Crush2048
{
    public class Board : MonoBehaviour
    {
        public static event Action OnCacheTileValues;
        public static event Action OnTileMatchEnded;
        public static event Action OnAssignTileValues;
        public static event Action OnGameOver;

        [SerializeField] Camera mainCamera;
        [SerializeField] GridSystem gridSystem;
        [SerializeField] MeshRenderer gridRenderer;
        [SerializeField] GameObject tilePrefab;
        [SerializeField] ObjectPool objectPool;

        public static bool CanTilesBeClicked { get; private set; } = true;

        private Sequence _tileMoveSequence;

        private const float BOARD_SIZE = 0.92f;

        private void OnEnable() => TileSwipe.OnTilesMatch += MatchTiles;

        private void OnDisable() => TileSwipe.OnTilesMatch -= MatchTiles;

        private void Awake()
        {
            InitializeBoardSize();
            gridSystem.InitializeGrid(gridRenderer);
            objectPool.InitializePool();
        }

        private void Start() => InitializeTiles();

        private void MatchTiles(SelectedTile firstSelectedTile, SelectedTile secondSelectedTile, BehaviourDelegate tileBehaviour)
        {
            Transform firstSelectedTransform = firstSelectedTile.TileObject.transform;
            Transform secondSelectedTransform = secondSelectedTile.TileObject.transform;

            CanTilesBeClicked = false;
            OnCacheTileValues?.Invoke();

            _tileMoveSequence = DOTween.Sequence().SetAutoKill(false);
            _tileMoveSequence.Append(firstSelectedTransform.DODynamicLookAt(secondSelectedTransform.position, 0.1f));
            _tileMoveSequence.Append(firstSelectedTransform.DOMove(secondSelectedTransform.position, 0.15f).SetEase(Ease.InBack));
            _tileMoveSequence.AppendCallback(() => tileBehaviour(firstSelectedTile, secondSelectedTile));
            _tileMoveSequence.AppendCallback(SpawnMissingTiles);
            _tileMoveSequence.AppendCallback(CheckPossibleMoves);
            _tileMoveSequence.AppendCallback(() => OnTileMatchEnded?.Invoke());
        }

        private void SpawnMissingTiles()
        {
            for (int i = 0; i < gridSystem.GridSize.Columns; i++)
            {
                List<Vector2Int> emptyCellsInColumn = gridSystem.GetEmptyCellsInColumn(i);
                List<TileToCellPair> tilesToBeAssigned = new List<TileToCellPair>();

                GetDesiredCellsForExistingTiles(i, emptyCellsInColumn, tilesToBeAssigned);
                SpawnNewTiles(i, emptyCellsInColumn, tilesToBeAssigned);

                AssignTilesToCells(tilesToBeAssigned);
            }
        }

        private void GetDesiredCellsForExistingTiles(int columnIndex, List<Vector2Int> emptyCellsInColumn, List<TileToCellPair> tilesToBeAssigned)
        {
            for (int j = 0; j < gridSystem.GridSize.Rows; j++)
            {
                GameObject tile = gridSystem.TilesAtGridCells[j, columnIndex];

                if (tile != null)
                {
                    List<Vector2Int> emptyCellsBeneath = emptyCellsInColumn.FindAll(cell => cell.x > j);

                    if (emptyCellsBeneath.Count == 0) continue;

                    Vector2Int desiredCell = new Vector2Int(j + emptyCellsBeneath.Count, columnIndex);
                    tilesToBeAssigned.Add(new TileToCellPair(tile, desiredCell));
                }
            }
        }

        private void SpawnNewTiles(int columnIndex, List<Vector2Int> emptyCellsInColumn, List<TileToCellPair> tilesToBeAssigned)
        {
            Vector3 firstGridCellPosition = gridSystem.GridCells[0, columnIndex];

            for (int j = 0; j < emptyCellsInColumn.Count; j++)
            {
                GameObject spawnedTile = objectPool.GetFromPool(Tags.Tile);

                Vector3 spawnPosition = new Vector3(firstGridCellPosition.x, firstGridCellPosition.y + (gridSystem.CellHeight * 1.4f * (j + 1)), tilePrefab.transform.position.z);
                spawnedTile.transform.position = spawnPosition;

                Vector2Int desiredCell = new Vector2Int(emptyCellsInColumn.Count - 1 - j, columnIndex);
                tilesToBeAssigned.Add(new TileToCellPair(spawnedTile, desiredCell));
            }
        }

        private void AssignTilesToCells(List<TileToCellPair> tilesToBeAssigned)
        {
            Vector3[,] gridCells = gridSystem.GridCells;

            foreach (TileToCellPair pair in tilesToBeAssigned)
            {
                Vector2Int cell = pair.Cell;
                GameObject tile = pair.Tile;

                float desiredYPosition = gridCells[cell.x, cell.y].y;
                tile.transform.DOMoveY(desiredYPosition, 0.2f).SetDelay(0.1f);
                gridSystem.AssignTileToCell(tile, cell);
            }
        }

        private void CheckPossibleMoves()
        {
            OnAssignTileValues?.Invoke();

            int rows = gridSystem.GridSize.Rows;
            int columns = gridSystem.GridSize.Columns;

            int[,] pointsWorthAtGridCells = gridSystem.PointsWorthAtCells;
            TileType[,] tileTypeAtCell = gridSystem.TileTypeAtCell;

            for (int i = 0; i < rows; i++)
            {
                for (int j = 0; j < columns; j++)
                {
                    int pointsWorthAtCell = pointsWorthAtGridCells[i, j];
                    bool isSpecial = tileTypeAtCell[i, j].isSpecial;

                    bool isntLastRow = i < rows - 1;
                    bool isntLastColumn = j < columns - 1;

                    bool areWorthSameInColumn = isntLastRow && pointsWorthAtGridCells[i + 1, j] == pointsWorthAtCell;
                    bool areWorthSameInRow = isntLastColumn && pointsWorthAtGridCells[i, j + 1] == pointsWorthAtCell;

                    bool areBothSpecialInColumn = isntLastRow && tileTypeAtCell[i + 1, j].isSpecial && isSpecial;
                    bool areBothSpecialInRow = isntLastColumn && tileTypeAtCell[i, j + 1].isSpecial && isSpecial;

                    bool isOneInColumnSpecial = isntLastRow && tileTypeAtCell[i + 1, j].isSpecial != isSpecial;
                    bool isOneInRowSpecial = isntLastColumn && tileTypeAtCell[i, j + 1].isSpecial != isSpecial;

                    bool canBeMergedInColumn = (!areBothSpecialInColumn && areWorthSameInColumn) || isOneInColumnSpecial;
                    bool canBeMergedInRow = (!areBothSpecialInRow && areWorthSameInRow) || isOneInRowSpecial;

                    if (canBeMergedInColumn || canBeMergedInRow)
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

            transform.localScale = Vector3.one * BOARD_SIZE * aspectRatio;

            float yPosition = mainCamera.transform.position.y - (aspectRatio + transform.position.y);
            Vector3 boardPosition = new Vector3(transform.position.x, yPosition, transform.position.z);
            transform.position = boardPosition;
        }
    }
}
