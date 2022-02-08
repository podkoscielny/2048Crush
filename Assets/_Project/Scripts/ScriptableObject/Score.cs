using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Crush2048
{
    [CreateAssetMenu(fileName = "Score", menuName = "ScriptableObjects/Score")]
    public class Score : ScriptableObject
    {
        public static event Action OnScoreUpdated;

        [SerializeField] int value;

        public int Value
        {
            get
            {
                return value;
            }
            private set
            {
                this.value = value;
                OnScoreUpdated?.Invoke();
            }
        }

        private int _cachedScore = 0;

        private void OnEnable()
        {
            //SceneController.OnSceneChange += ResetScore;
            Board.OnCacheTileValues += CacheScore;
            Board.OnGameRestart += ResetScore;
            Board.OnCachedValuesLoaded += LoadCachedScore;
            MoveReverse.OnTilesReverse += ReverseScoreToCached;

#if UNITY_EDITOR
            EditorApplication.playModeStateChanged += ResetValuesOnEditorQuit;
#endif
        }

        private void OnDisable()
        {
            //SceneController.OnSceneChange -= ResetScore;
            Board.OnCacheTileValues -= CacheScore;
            Board.OnGameRestart -= ResetScore;
            Board.OnCachedValuesLoaded -= LoadCachedScore;
            MoveReverse.OnTilesReverse -= ReverseScoreToCached;
            ResetScore();

#if UNITY_EDITOR
            EditorApplication.playModeStateChanged -= ResetValuesOnEditorQuit;
#endif
        }

        private void OnValidate()
        {
#if UNITY_EDITOR
            if (EditorApplication.isPlaying)
            {
                Value = Mathf.Max(value, 0);
            }
            else
            {
                value = 0;
            }
#endif
        }

        public void AddPoints(int pointsToAdd) => Value += pointsToAdd;

        private void ResetScore() => Value = 0;

        private void CacheScore() => _cachedScore = value;

        private void ReverseScoreToCached() => Value = _cachedScore;

        private void LoadCachedScore(CachedBaord cachedBoard) => Value = cachedBoard.Score;


#if UNITY_EDITOR
        private void ResetValuesOnEditorQuit(PlayModeStateChange changedState)
        {
            if (changedState == PlayModeStateChange.ExitingPlayMode)
            {
                ResetScore();
            }
        }
#endif
    }
}