using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AoOkami.SaveSystem;

namespace Crush2048
{
    public class BoardCacher : MonoBehaviour
    {
        public static event Action<CachedBoard> OnCachedValuesLoaded;

        [SerializeField] Score score;
        [SerializeField] GridSystem gridSystem;
        [SerializeField] IntValue reversesLeft;

        private string _savePath = "";

        private void OnEnable()
        {
            Board.OnBoardCached += CacheCurrentBoard;
            GameInitializer.OnTilesInitialized += LoadCachedBoard;
            GameRestart.OnGameRestart += DeleteSaveFile;
        }

        private void OnDisable()
        {
            Board.OnBoardCached -= CacheCurrentBoard;
            GameInitializer.OnTilesInitialized -= LoadCachedBoard;
            GameRestart.OnGameRestart -= DeleteSaveFile;
        }

        private void Awake() => CacheSavePath();

        private void LoadCachedBoard()
        {
            CachedBoard cachedBoard = SaveSystem.Load<CachedBoard>(_savePath);
            bool isBoardCached = cachedBoard.tileTypesAtCells != null;

            reversesLeft.Value = isBoardCached ? cachedBoard.moveReversesLeft : reversesLeft.BaseValue;

            if (isBoardCached)
            {
                if (cachedBoard.cachedTileTypesAtCells.Length > 0 && cachedBoard.cachedPointsAtCells.Length > 0)
                {
                    TileType[] tileTypeVariants = gridSystem.GridSize.GetTileTypeVariants();
                    TileType[,] cachedTileTypes = TileTypeConverter.SerializableArrayToNormal(cachedBoard.cachedTileTypesAtCells, tileTypeVariants);
                    int[,] cachedPointsWorth = cachedBoard.cachedPointsAtCells;

                    gridSystem.SetCachedArrays(cachedTileTypes, cachedPointsWorth);
                }

                OnCachedValuesLoaded?.Invoke(cachedBoard);
            }
        }

        private void CacheCurrentBoard(bool isGameOver)
        {
            CachedTileType[,] tileTypes = TileTypeConverter.NormalArrayToSerializable(gridSystem.TileTypeAtCell);
            CachedTileType[,] cachedTileTypes = TileTypeConverter.NormalArrayToSerializable(gridSystem.CachedTilesAtCells);
            CachedBoard cachedBoard = GetNewCachedBoard(tileTypes, cachedTileTypes, isGameOver);

            SaveSystem.Save<CachedBoard>(_savePath, cachedBoard);
        }

        private CachedBoard GetNewCachedBoard(CachedTileType[,] tileTypes, CachedTileType[,] cachedTileTypes, bool isGameOver)
        {
            CachedBoard cachedBoard = new CachedBoard();

            cachedBoard.score = score.Value;
            cachedBoard.cachedScore = score.CachedScore;
            cachedBoard.moveReversesLeft = reversesLeft.Value;
            cachedBoard.tileTypesAtCells = tileTypes;
            cachedBoard.cachedTileTypesAtCells = cachedTileTypes;
            cachedBoard.pointsAtCells = gridSystem.PointsWorthAtCells;
            cachedBoard.cachedPointsAtCells = gridSystem.CachedPointsWorthAtCells;
            cachedBoard.isGameOver = isGameOver;

            return cachedBoard;
        }

        private void DeleteSaveFile() => SaveSystem.DeleteSaveFile(_savePath);

        private void CacheSavePath() => _savePath = $"{gridSystem.GridSize.name}_board";
    }
}
