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
        [SerializeField] IntValue reversesLeft;

        private bool _areTilesCached = true;

        private void OnEnable()
        {
            Board.OnTileMatchEnded += ReenableReversing;
            GameRestart.OnGameRestart += InitializeReversesCount;
            reversesLeft.OnValueChanged += SetReversesText;
        }

        private void OnDisable()
        {
            Board.OnTileMatchEnded -= ReenableReversing;
            GameRestart.OnGameRestart -= InitializeReversesCount;
            reversesLeft.OnValueChanged -= SetReversesText;
        }

        public void ReverseMove()
        {
            if (CantBeReversed()) return;

            _areTilesCached = false;

            reversesLeft.Value = Mathf.Max(0, reversesLeft.Value - 1);
            SetReversesText();

            OnTilesReverse?.Invoke();
        }

        private void ReenableReversing() => _areTilesCached = true;

        private bool CantBeReversed() => !_areTilesCached || reversesLeft.Value <= 0 || !Board.CanTilesBeClicked || score.Value <= 0;

        private void InitializeReversesCount()
        {
            reversesLeft.Value = reversesLeft.BaseValue;
            SetReversesText();
        }

        private void SetReversesText() => remainingReversesText.text = $"{reversesLeft.Value}";
    }
}
