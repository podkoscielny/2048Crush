using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Tags = MultipleTagSystem.TagSystem.Tags;

namespace Crush2048
{
    public class GameInitializer : MonoBehaviour
    {
        public static event Action OnTilesInitialized;

        [SerializeField] Camera mainCamera;
        [SerializeField] MeshRenderer gridRenderer;
        [SerializeField] GameObject tilePrefab;
        [SerializeField] GridSystem gridSystem;
        [SerializeField] ObjectPool objectPool;

        private const float BOARD_SIZE = 0.92f;

        private void OnEnable() => GameRestart.OnGameRestart += InitializeTiles;
        private void OnDisable() => GameRestart.OnGameRestart -= InitializeTiles;

        private void Awake()
        {
            InitializeBoardSize();
            gridSystem.InitializeGrid(gridRenderer);
            objectPool.InitializePool();
        }

        private void Start() => InitializeTiles();

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

            OnTilesInitialized?.Invoke();
        }
    }
}
