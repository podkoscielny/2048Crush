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

        private void Start() => SetReversesRemainingText();

        public void ReverseMove()
        {
            if (CantBeReversed()) return;

            _reversesLeft = Mathf.Max(0, _reversesLeft - 1);
            SetReversesRemainingText();

            OnTilesReverse?.Invoke();
        }

        private bool CantBeReversed() => _reversesLeft <= 0 || !Board.CanTilesBeClicked || score.Value <= 0;

        private void SetReversesRemainingText() => remainingReversesText.text = $"Left: {_reversesLeft}";
    }
}
