using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEditor;

namespace Crush2048
{
    public class SceneController : MonoBehaviour
    {
        public static event Action OnSceneChanged;

        public void GoToPlayScene() => StartCoroutine(LoadScene("Game"));

        public void GoToMainMenu() => StartCoroutine(LoadScene("Menu"));

        private IEnumerator LoadScene(string sceneName)
        {
            OnSceneChanged?.Invoke();

            MasterVolume.MuteTest(() => SceneManager.LoadSceneAsync(sceneName));

            yield return null;
        }

        public void QuitGame()
        {
#if UNITY_EDITOR
            EditorApplication.isPlaying = false;
#else
            Application.Quit();

#endif
        }
    }
}
