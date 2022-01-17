using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Crush2048
{
    public class GameOverPanel : MonoBehaviour
    {
        [SerializeField] GameObject gameOverPanel;

        private void OnEnable() => Board.OnGameOver += ShowGameOverPanel;

        private void OnDisable() => Board.OnGameOver -= ShowGameOverPanel;

        private void ShowGameOverPanel() => gameOverPanel.SetActive(true);
    } 
}
