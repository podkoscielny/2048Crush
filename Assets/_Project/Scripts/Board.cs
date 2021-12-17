using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Tags = TagSystem.Tags;

public class Board : MonoBehaviour
{
    [SerializeField] Camera mainCamera;
    [SerializeField] GridSystem gridSystem;
    [SerializeField] MeshRenderer gridRenderer;
    [SerializeField] GameObject tilePrefab;
    [SerializeField] ObjectPool objectPool;
    [SerializeField] Score score;

    private void OnEnable()
    {
        
    }

    private void OnDisable()
    {
        
    }

    private void Awake()
    {
        InitializeBoardSize();
        gridSystem.InitializeGrid(gridRenderer);
        objectPool.InitializePool();
    }

    private void Start()
    {
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
