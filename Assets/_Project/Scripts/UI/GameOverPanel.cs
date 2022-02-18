using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Crush2048
{
    public class GameOverPanel : MonoBehaviour
    {
        [SerializeField] GameObject gameOverPanel;

        private void OnEnable()
        {
            Board.OnGameOver += ShowGameOverPanel;
            BoardCacher.OnCachedValuesLoaded += ShowGameOverPanel;
            Board.OnGameRestart += HideGameOverPanel;
            MoveReverse.OnTilesReverse += HideGameOverPanel;
        }

        private void OnDisable()
        {
            Board.OnGameOver -= ShowGameOverPanel;
            BoardCacher.OnCachedValuesLoaded -= ShowGameOverPanel;
            Board.OnGameRestart -= HideGameOverPanel;
            MoveReverse.OnTilesReverse -= HideGameOverPanel;
        }

        private void ShowGameOverPanel() => gameOverPanel.SetActive(true);

        private void HideGameOverPanel() => gameOverPanel.SetActive(false);

        private void ShowGameOverPanel(CachedBoard cachedBoard) => gameOverPanel.SetActive(cachedBoard.isGameOver);
    } 
}
