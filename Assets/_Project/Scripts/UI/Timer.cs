using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Timer : MonoBehaviour
{
    public static event Action OnTimesUp;

    [SerializeField] TextMeshProUGUI timerText;

    private int _remainingTimeInSeconds = 120;

    private void Start()
    {
        UpdateTimerText();
        StartCoroutine(Countdown());
    }

    private IEnumerator Countdown()
    {
        while (_remainingTimeInSeconds > 0)
        {
            yield return new WaitForSeconds(1f);

            _remainingTimeInSeconds--;
            UpdateTimerText();
        }

        OnTimesUp?.Invoke();
    }

    private void UpdateTimerText()
    {
        float minutes = Mathf.Floor(_remainingTimeInSeconds / 60f);
        int seconds = Mathf.RoundToInt(_remainingTimeInSeconds % 60);

        string secondsText = seconds < 10 ? $"0{seconds}" : $"{seconds}";

        timerText.text = $"{minutes}:{secondsText}";
    }
}