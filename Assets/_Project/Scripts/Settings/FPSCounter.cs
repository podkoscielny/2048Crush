using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FPSCounter : MonoBehaviour
{
    private string _label = "";
    private float _fpsCount;

    private IEnumerator Start()
    {
        GUI.depth = 2;
        while (true)
        {
            if (Time.timeScale == 1)
            {
                yield return new WaitForSeconds(0.1f);
                _fpsCount = (1 / Time.deltaTime);
                _label = "FPS :" + (Mathf.Round(_fpsCount));
            }
            else
            {
                _label = "Pause";
            }
            yield return new WaitForSeconds(0.5f);
        }
    }

    void OnGUI()
    {
        GUI.Label(new Rect(5, 40, 100, 25), _label);
    }
}
