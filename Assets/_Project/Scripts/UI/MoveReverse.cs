using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

namespace Crush2048
{
    public class MoveReverse : MonoBehaviour
    {
        public static event Action OnTilesReverse;

        [SerializeField] TextMeshProUGUI remainingReversesText;
        [SerializeField] Score score;

        private int _reversesLeft;
        private bool _areTilesCached = false;

        private const int MAX_REVERSES = 3;

        private void OnEnable()
        {
            Board.OnTileMatchEnded += ReenableReversing;
            Board.OnGameRestart += InitializeReversesCount;
        }

        private void OnDisable()
        {
            Board.OnTileMatchEnded -= ReenableReversing;
            Board.OnGameRestart += InitializeReversesCount;
        }

        private void Start() => InitializeReversesCount();

        public void ReverseMove()
        {
            if (CantBeReversed()) return;

            _areTilesCached = false;

            _reversesLeft = Mathf.Max(0, _reversesLeft - 1);
            SetReversesText();

            OnTilesReverse?.Invoke();
        }

        private void ReenableReversing() => _areTilesCached = true;

        private bool CantBeReversed() => !_areTilesCached || _reversesLeft <= 0 || !Board.CanTilesBeClicked || score.Value <= 0;

        private void InitializeReversesCount()
        {
            _reversesLeft = MAX_REVERSES;
            SetReversesText();
        }

        private void SetReversesText() => remainingReversesText.text = $"{_reversesLeft}";
    }
}
