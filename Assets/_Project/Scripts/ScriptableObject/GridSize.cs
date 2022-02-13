using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Crush2048
{
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

        private TileType[] _tileTypeVariants = new TileType[0];

        private void OnValidate()
        {
            ClampRowsAndColumnsCount();
            ClampTileProbability();
        }

        private void OnEnable() => CalculateProbabilitySum();

        public TileType[] GetTileTypeVariants()
        {
            _tileTypeVariants = new TileType[TileTypes.Length];

            for (int i = 0; i < TileTypes.Length; i++)
            {
                _tileTypeVariants[i] = TileTypes[i].tileType;
            }

            return _tileTypeVariants;
        }

        private void ClampRowsAndColumnsCount()
        {
            rows = Mathf.Max(1, rows);
            columns = Mathf.Max(1, columns);
        }

        private void ClampTileProbability()
        {
            for (int i = 0; i < tileTypes.Length; i++)
            {
                tileTypes[i].spawnProbability = Mathf.Clamp(tileTypes[i].spawnProbability, 0, 1);
            }
        }

        private void SortTileTypesByProbability() => Array.Sort(tileTypes, (x, y) => x.spawnProbability.CompareTo(y.spawnProbability));

        private void CalculateProbabilitySum()
        {
            float probability = 0;

            foreach (TileProbabilityPair type in tileTypes)
            {
                probability += type.spawnProbability;
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
}
