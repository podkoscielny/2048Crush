using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Crush2048
{
    public class BackButtonHandler : MonoBehaviour
    {
        [SerializeField] MeshCollider clickBlocker;
        [SerializeField] PopupOpener popupOpener;

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                clickBlocker.enabled = true;
                popupOpener.ShowPopup();
            }
        }
    }
}
