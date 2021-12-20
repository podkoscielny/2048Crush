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
    [SerializeField] TileProbabilityPair[] tileTypes;

    public int Rows => rows;
    public int Columns => columns;
    public TileProbabilityPair[] TileTypes => tileTypes;
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
            tileTypes[i].probability = Mathf.Clamp(tileTypes[i].probability, 0, 1);
        }
    }

    private void SortTileTypesByProbability() => Array.Sort(tileTypes, (x, y) => x.probability.CompareTo(y.probability));

    private void CalculateProbabilitySum()
    {
        float probability = 0;

        foreach (var type in tileTypes)
        {
            probability += type.probability;
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