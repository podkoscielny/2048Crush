using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Crush2048
{
    [ExecuteInEditMode]
    public class BackgroundChanger : MonoBehaviour
    {
        [SerializeField] ThemeManager themeManager;
        [SerializeField] Image backgroundImage;

        private void OnEnable() => ThemeManager.OnThemeChanged += ChangeBackgroundImage;

        private void OnDisable() => ThemeManager.OnThemeChanged -= ChangeBackgroundImage;

        private void Start() => ChangeBackgroundImage();

        private void ChangeBackgroundImage() => backgroundImage.sprite = themeManager.ThemeSelected.BackgroundImage;
    }
}
