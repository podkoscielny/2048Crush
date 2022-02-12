using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEditor;

namespace Crush2048
{
    public class SceneController : MonoBehaviour
    {
        public void GoToPlayScene() => SceneManager.LoadScene("Game");

        public void GoToMainMenu() => SceneManager.LoadScene("Menu");

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
