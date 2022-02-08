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

        private int _highscore = 0;
        private string _savePath = "";

        private void OnEnable() => Score.OnScoreUpdated += UpdateScoreText;

        private void OnDisable() => Score.OnScoreUpdated -= UpdateScoreText;

        private void Awake() => CacheSavePath();

        private void Start() => InitializeScores();

        private void UpdateScoreText()
        {
            currentScoreText.text = score.Value.ToString();

            if (score.Value > _highscore) SetNewHighscore();
        }

        private void SetNewHighscore()
        {
            SaveSystem.Save<int>(_savePath, score.Value);
            _highscore = score.Value;
            highscoreText.text = score.Value.ToString();
        }

        private void InitializeScores()
        {
            _highscore = SaveSystem.Load<int>(_savePath);

            currentScoreText.text = score.Value.ToString();
            highscoreText.text = _highscore.ToString();
        }

        private void CacheSavePath() => _savePath = $"{gridSystem.GridSize.name}_score";
    }
}
