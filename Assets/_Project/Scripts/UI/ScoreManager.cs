using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

namespace Crush2048
{
    public class ScoreManager : MonoBehaviour
    {
        [SerializeField] TextMeshProUGUI currentScoreText;
        [SerializeField] TextMeshProUGUI highscoreText;
        [SerializeField] GridSystem gridSystem;
        [SerializeField] Score score;
        [SerializeField] Highscore highscore;

        private void OnEnable() => Score.OnScoreUpdated += UpdateScoreText;

        private void OnDisable() => Score.OnScoreUpdated -= UpdateScoreText;

        private void Start() => InitializeScores();

        private void UpdateScoreText()
        {
            string scoreToString = score.Value.ToString();
            currentScoreText.text = scoreToString;

            if (score.Value > highscore.Value)
            {
                SaveSystem.Save(gridSystem.GridSize.name, score.Value);
                highscore.SetValue(score.Value);
                highscoreText.text = scoreToString;
            }
        }

        private void InitializeScores()
        {
            int cachedHighscore = SaveSystem.Load(gridSystem.GridSize.name);

            currentScoreText.text = score.Value.ToString();
            highscoreText.text = cachedHighscore.ToString();
        }
    }
}
