using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Crush2048
{
    public class SceneController : MonoBehaviour
    {
        public void GoToPlayScene() => SceneManager.LoadScene("Main");
    }
}
