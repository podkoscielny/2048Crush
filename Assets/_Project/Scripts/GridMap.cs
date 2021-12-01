using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridMap : MonoBehaviour
{
    [SerializeField] GridSystem gridSystem;
    [SerializeField] MeshRenderer gridRenderer;

    private List<Vector3> _cellPoints = new List<Vector3>();
    private Vector3 _cubeSize = new Vector3(0.5f, 0.5f, 0.5f);

    void Start()
    {
        InitializeGrid();
    }

    void OnDrawGizmos()
    {
        if (_cellPoints.Count > 0)
        {
            foreach (Vector3 point in _cellPoints)
            {
                Gizmos.DrawWireCube(point, _cubeSize);
            }
        }
    }

    private void InitializeGrid()
    {
        float gridWidth = gridRenderer.bounds.size.x;
        float gridHeight = gridRenderer.bounds.size.y;
        Vector3 gridCenter = gridRenderer.bounds.center;

        float cellWidth = gridWidth / gridSystem.Rows;
        float cellHeight = gridHeight / gridSystem.Columns;

        _cubeSize = new Vector3(cellWidth, cellHeight, 0.1f);

        for (int i = 0; i < gridSystem.Rows; i++)
        {
            float xPosition = gridCenter.x - (gridWidth / 2) + (cellWidth / 2) + (i * cellWidth);

            for (int j = 0; j < gridSystem.Columns; j++)
            {
                float yPosition = gridCenter.y - (gridHeight / 2) + (cellHeight / 2) + (j * cellHeight);

                _cellPoints.Add(new Vector3(xPosition, yPosition, gridCenter.z));
            }
        }
    }
}