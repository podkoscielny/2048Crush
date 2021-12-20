using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
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
    public float ProbabilitySum { get; private set; }

    void OnValidate()
    {
        ClampRowsAndColumnsCount();
        ClampTileTypeProbability();
    }

    void OnEnable() => CalculateProbabilitySum();

    private void ClampRowsAndColumnsCount()
    {
        rows = Mathf.Max(1, rows);
        columns = Mathf.Max(1, columns);
    }

    private void ClampTileTypeProbability()
    {
        for (int i = 0; i < tileTypes.Length; i++)
        {
            tileTypes[i].value = Mathf.Clamp(tileTypes[i].value, 0, 1);
        }
    }

    private void SortTileTypesByProbability() => Array.Sort(tileTypes, (x, y) => x.value.CompareTo(y.value));

    private void CalculateProbabilitySum()
    {
        float probability = 0;

        foreach (var type in tileTypes)
        {
            probability += type.value;
        }

        ProbabilitySum = probability;
    }

#if UNITY_EDITOR
    [CustomEditor(typeof(GridSize))]
    class CustomTypeEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            GridSize gridSize = (GridSize)target;

            if (!EditorGUIUtility.editingTextField)
            {
                gridSize.SortTileTypesByProbability();
                gridSize.CalculateProbabilitySum();
            }
        }
    }

#endif
}