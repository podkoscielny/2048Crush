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

        private int _reversesLeft = 3;
        private bool _areTilesCached = false;

        private void OnEnable() => Board.OnTileMatchEnded += ReenableReversing;

        private void OnDisable() => Board.OnTileMatchEnded -= ReenableReversing;

        private void Start() => SetReversesRemainingText();

        public void ReverseMove()
        {
            if (CantBeReversed()) return;

            _reversesLeft = Mathf.Max(0, _reversesLeft - 1);
            SetReversesRemainingText();

            OnTilesReverse?.Invoke();
        }

        private void ReenableReversing() => _areTilesCached = true;

        private bool CantBeReversed() => !_areTilesCached || _reversesLeft <= 0 || !Board.CanTilesBeClicked || score.Value <= 0;

        private void SetReversesRemainingText() => remainingReversesText.text = $"{_reversesLeft}";
    }
}
