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

    private const int MINIMUM_POINTS_WORTH = 2;

    void OnValidate()
    {
        ClampRowsAndColumnsCount();
        ClampTileProbabilityAndPoints();
    }

    void OnEnable() => CalculateProbabilitySum();

    private void ClampRowsAndColumnsCount()
    {
        rows = Mathf.Max(1, rows);
        columns = Mathf.Max(1, columns);
    }

    private void ClampTileProbabilityAndPoints()
    {
        for (int i = 0; i < tileTypes.Length; i++)
        {
            tileTypes[i].probability = Mathf.Clamp(tileTypes[i].probability, 0, 1);
            tileTypes[i].pointsWorth = tileTypes[i].pointsWorth % 2 == 0 ? Mathf.Max(2, tileTypes[i].pointsWorth) : MINIMUM_POINTS_WORTH;
        }
    }

    private void SortTileTypesByProbability() => Array.Sort(tileTypes, (x, y) => x.probability.CompareTo(y.probability));

    private void CalculateProbabilitySum()
    {
        float probability = 0;

        foreach (TileProbabilityPair type in tileTypes)
        {
            probability += type.probability;
        }

        ProbabilitySum = probability;
    }

#if UNITY_EDITOR
    [CustomEditor(typeof(GridSize))]
    class GridSizeEditor : Editor
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