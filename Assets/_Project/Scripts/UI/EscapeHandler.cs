using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Crush2048
{
    public class EscapeHandler : MonoBehaviour
    {
        [SerializeField] UnityEvent OnEscaped;

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Escape)) OnEscaped.Invoke();
        }
    }
}
