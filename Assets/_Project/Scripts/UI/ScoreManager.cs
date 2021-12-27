using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ScoreManager : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI currentScoreText;
    [SerializeField] TextMeshProUGUI highscoreText;
    [SerializeField] Score score;
    [SerializeField] Highscore highscore;

    private void OnEnable()
    {
        Score.OnScoreUpdated += UpdateScoreText;
    }

    private void OnDisable()
    {
        Score.OnScoreUpdated -= UpdateScoreText;
    }

    private void UpdateScoreText()
    {
        currentScoreText.text = score.Value.ToString();

        if (score.Value > highscore.Value)
        {
            highscore.SetValue(score.Value);
            highscoreText.text = highscore.Value.ToString();
        }
    }
}