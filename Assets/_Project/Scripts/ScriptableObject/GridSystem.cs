using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "GridSystem", menuName = "ScriptableObjects/GridSystem")]
public class GridSystem : ScriptableObject
{
    [SerializeField] GridSize gridSize;

    public GridSize GridSize => gridSize;

    public void SetGridSize(GridSize newGridSize) => gridSize = newGridSize;
}