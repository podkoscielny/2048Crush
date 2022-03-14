using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEditor;

namespace Crush2048
{
    public class SceneController : MonoBehaviour
    {
        private float _volumeRef;
        private const float LOAD_DELAY = 0.5f;

        public void GoToPlayScene() => StartCoroutine(LoadScene("Game"));

        public void GoToMainMenu() => StartCoroutine(LoadScene("Menu"));

        private IEnumerator LoadScene(string sceneName)
        {
            StartCoroutine(MuteMasterVolume());

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

        private IEnumerator MuteMasterVolume()
        {
            while (AudioListener.volume > 0.01f)
            {
                AudioListener.volume = Mathf.SmoothDamp(AudioListener.volume, 0, ref _volumeRef, 0.2f);
                yield return null;
            }
        }
    }
}
