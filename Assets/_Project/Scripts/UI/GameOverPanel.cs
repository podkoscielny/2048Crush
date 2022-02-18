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
            TileMatchSequence.OnGameOver += ShowGameOverPanel;
            BoardCacher.OnCachedValuesLoaded += ShowGameOverPanel;
            GameRestart.OnGameRestart += HideGameOverPanel;
            MoveReverse.OnTilesReverse += HideGameOverPanel;
        }

        private void OnDisable()
        {
            TileMatchSequence.OnGameOver -= ShowGameOverPanel;
            BoardCacher.OnCachedValuesLoaded -= ShowGameOverPanel;
            GameRestart.OnGameRestart -= HideGameOverPanel;
            MoveReverse.OnTilesReverse -= HideGameOverPanel;
        }

        private void ShowGameOverPanel() => gameOverPanel.SetActive(true);

        private void HideGameOverPanel() => gameOverPanel.SetActive(false);

        private void ShowGameOverPanel(CachedBoard cachedBoard) => gameOverPanel.SetActive(cachedBoard.isGameOver);
    } 
}
