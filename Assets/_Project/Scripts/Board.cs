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
        public static event Action OnTilesReverse;
        public static event Action OnGameOver;

        [SerializeField] Camera mainCamera;
        [SerializeField] GridSystem gridSystem;
        [SerializeField] MeshRenderer gridRenderer;
        [SerializeField] GameObject tilePrefab;
        [SerializeField] ObjectPool objectPool;

        public static bool CanTilesBeClicked { get; private set; } = true;

        private Sequence _tileMoveSequence;

        private void OnEnable() => TileSwipe.OnTilesMatch += MatchTiles;

        private void OnDisable() => TileSwipe.OnTilesMatch -= MatchTiles;

        private void Awake()
        {
            InitializeBoardSize();
            gridSystem.InitializeGrid(gridRenderer);
            objectPool.InitializePool();
        }

        private void Start() => InitializeTiles();

        public void ReverseMove()
        {
            if (!CanTilesBeClicked) return;

            OnTilesReverse?.Invoke();
        }

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
                List<Vector2Int> emptyCellsInColumn = new List<Vector2Int>();
                List<TileToCellPair> tilesToBeAssigned = new List<TileToCellPair>();

                for (int j = 0; j < gridSystem.GridSize.Rows; j++)
                {
                    if (gridSystem.TilesAtGridCells[j, i] == null) emptyCellsInColumn.Add(new Vector2Int(j, i));
                }

                for (int j = 0; j < gridSystem.GridSize.Rows; j++)
                {
                    GameObject tile = gridSystem.TilesAtGridCells[j, i];

                    if (tile != null)
                    {
                        List<Vector2Int> emptyCellsBeneath = emptyCellsInColumn.FindAll(cell => cell.x > j);

                        if (emptyCellsBeneath.Count == 0) continue;

                        Vector2Int desiredCell = new Vector2Int(j + emptyCellsBeneath.Count, i);
                        float desiredYPosition = gridSystem.GridCells[desiredCell.x, desiredCell.y].y;

                        tile.transform.DOMoveY(desiredYPosition, 0.2f).SetDelay(0.1f);
                        tilesToBeAssigned.Add(new TileToCellPair(tile, desiredCell));
                    }
                }

                Vector3 firstGridCellPosition = gridSystem.GridCells[0, i];

                for (int j = 0; j < emptyCellsInColumn.Count; j++)
                {
                    GameObject spawnedTile = objectPool.GetFromPool(Tags.Tile);

                    Vector3 spawnPosition = new Vector3(firstGridCellPosition.x, firstGridCellPosition.y + (gridSystem.CellHeight * 1.4f * (j + 1)), tilePrefab.transform.position.z);
                    spawnedTile.transform.position = spawnPosition;

                    Vector2Int desiredCell = new Vector2Int(emptyCellsInColumn.Count - 1 - j, i);
                    float desiredYPosition = gridSystem.GridCells[desiredCell.x, desiredCell.y].y;
                    spawnedTile.transform.DOMoveY(desiredYPosition, 0.2f).SetDelay(0.1f);

                    tilesToBeAssigned.Add(new TileToCellPair(spawnedTile, desiredCell));
                }

                AssignTilesToCells(tilesToBeAssigned);
                emptyCellsInColumn.Clear();
            }
        }

        private void AssignTilesToCells(List<TileToCellPair> tilesToBeAssigned)
        {
            foreach (TileToCellPair pair in tilesToBeAssigned)
            {
                gridSystem.AssignTileToCell(pair.Tile, pair.Cell);
            }
        }

        private void CheckPossibleMoves()
        {
            CanTilesBeClicked = true;
            //OnAssignPointsWorthToCells?.Invoke();

            //int rows = gridSystem.GridSize.Rows;
            //int columns = gridSystem.GridSize.Columns;

            //for (int i = 0; i < rows; i++)
            //{
            //    for (int j = 0; j < columns; j++)
            //    {
            //        int pointsWorthAtCell = gridSystem.PointsWorthAtGridCells[i, j];

            //        bool canBeMargedInColumn = i < rows - 1 && gridSystem.PointsWorthAtGridCells[i + 1, j] == pointsWorthAtCell;
            //        bool canBeMargedInRow = j < columns - 1 && gridSystem.PointsWorthAtGridCells[i, j + 1] == pointsWorthAtCell;

            //        if (canBeMargedInColumn || canBeMargedInRow)
            //        {
            //            CanTilesBeClicked = true;
            //            return;
            //        }
            //    }
            //}

            //EndGame();
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
}
