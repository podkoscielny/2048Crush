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
            Board.OnCachedValuesLoaded += ShowGameOverPanel;
        }

        private void OnDisable()
        {
            Board.OnGameOver -= ShowGameOverPanel;
            Board.OnCachedValuesLoaded -= ShowGameOverPanel;
        }

        private void ShowGameOverPanel() => gameOverPanel.SetActive(true);

        private void ShowGameOverPanel(CachedBoard cachedBoard) => gameOverPanel.SetActive(cachedBoard.isGameOver);
    } 
}
