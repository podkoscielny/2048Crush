using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEditor;

namespace Crush2048
{
    public class SceneController : MonoBehaviour
    {
        private const float LOAD_DELAY = 0.5f;

        public void GoToPlayScene() => StartCoroutine(LoadScene("Game"));

        public void GoToMainMenu() => StartCoroutine(LoadScene("Menu"));

        private IEnumerator LoadScene(string sceneName)
        {
            yield return new WaitForSeconds(LOAD_DELAY);

            SceneManager.LoadSceneAsync(sceneName);
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
