using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using DG.Tweening;

namespace Crush2048
{
    public class GridSizeChanger : MonoBehaviour
    {
        [SerializeField] TextMeshProUGUI gridSizeText;
        [SerializeField] GridSystem gridSystem;
        [SerializeField] GridSize[] gridSizes;

        private GridSize _currentGridSize => gridSizes[_selectedGridSizeIndex];

        private int _selectedGridSizeIndex = 0;
        private Vector3 _initialTextScale;
        private Sequence _punchScaleSequence;

        private const float ANIMATION_DURATION = 0.15f;
        private const float ENLARGED_FACTOR = 1.4f;

        private void OnValidate() => SortGridSizesByRows();

        private void Awake() => _initialTextScale = gridSizeText.rectTransform.localScale;

        private void Start() => SetGridSize();

        public void DecreaseGridSize()
        {
            int previousIndex = _selectedGridSizeIndex - 1;

            if (previousIndex < 0) return;

            _selectedGridSizeIndex = Mathf.Max(0, _selectedGridSizeIndex - 1);

            SetGridSize();
            PunchScaleText();
        }

        public void IncreaseGridSize()
        {
            int nextIndex = _selectedGridSizeIndex + 1;

            if (nextIndex > gridSizes.Length - 1) return;

            _selectedGridSizeIndex = Mathf.Min(nextIndex, gridSizes.Length - 1);

            SetGridSize();
            PunchScaleText();
        }

        private void SortGridSizesByRows() => Array.Sort(gridSizes, (x, y) => x.Rows.CompareTo(y.Rows));

        private void SetGridSize()
        {
            gridSystem.SetGridSize(_currentGridSize);
            gridSizeText.SetText($"{_currentGridSize.Rows}X{_currentGridSize.Columns}");
        }

        private void PunchScaleText()
        {
            _punchScaleSequence = DOTween.Sequence().SetAutoKill(false);

            _punchScaleSequence.Append(gridSizeText.rectTransform.DOScale(_initialTextScale * ENLARGED_FACTOR, ANIMATION_DURATION).SetEase(Ease.OutCirc));
            _punchScaleSequence.Append(gridSizeText.rectTransform.DOScale(_initialTextScale, ANIMATION_DURATION).SetEase(Ease.OutCirc));
        }
    }
}
