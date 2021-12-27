using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class GridSizeChanger : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI gridSizeText;
    [SerializeField] GridSystem gridSystem;
    [SerializeField] GridSize[] gridSizes;

    private GridSize _currentGridSize => gridSizes[_selectedGridSizeIndex];

    private int _selectedGridSizeIndex = 0;

    private void OnValidate() => SortGridSizesByRows();

    private void Start() => SetGridSize();

    public void DecreaseGridSize()
    {
        _selectedGridSizeIndex = Mathf.Max(0, _selectedGridSizeIndex - 1);
        SetGridSize();
    }

    public void IncreaseGridSize()
    {
        _selectedGridSizeIndex = Mathf.Min(_selectedGridSizeIndex + 1, gridSizes.Length - 1);
        SetGridSize();
    }

    private void SortGridSizesByRows() => Array.Sort(gridSizes, (x, y) => x.Rows.CompareTo(y.Rows));

    private void SetGridSize()
    {
        gridSystem.SetGridSize(_currentGridSize);
        gridSizeText.SetText($"{_currentGridSize.Rows}X{_currentGridSize.Columns}");
    }
}
