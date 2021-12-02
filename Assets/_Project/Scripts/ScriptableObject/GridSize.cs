using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "GridSize", menuName = "ScriptableObjects/GridSize")]
public class GridSize : ScriptableObject
{
    [SerializeField] int rows;
    [SerializeField] int columns;

    public int Rows => rows;
    public int Columns => columns;

    void OnValidate()
    {
        rows = Mathf.Max(1, rows);
        columns = Mathf.Max(1, columns);
    }
}