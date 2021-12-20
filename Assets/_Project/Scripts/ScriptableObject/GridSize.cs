using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "GridSize", menuName = "ScriptableObjects/GridSize")]
public class GridSize : ScriptableObject
{
    [SerializeField] int rows;
    [SerializeField] int columns;
    [SerializeField] KeyValuePair<TileType, float>[] tileTypes;

    public int Rows => rows;
    public int Columns => columns;
    public KeyValuePair<TileType, float>[] TileTypes => tileTypes;

    void OnValidate()
    {
        rows = Mathf.Max(1, rows);
        columns = Mathf.Max(1, columns);

        for (int i = 0; i < tileTypes.Length; i++)
        {
            tileTypes[i].value = Mathf.Clamp(tileTypes[i].value, 0, 1);
        }
    }
}