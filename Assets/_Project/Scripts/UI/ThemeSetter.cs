using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

namespace Crush2048
{
    public class ThemeSetter : MonoBehaviour
    {
        [SerializeField] ThemeManager themeManager;
        [SerializeField] Image themeImage;
        [SerializeField] TextMeshProUGUI themeText;

        private void Start() => SetCurrentThemeUI(themeManager.ThemeSelected);

        public void SetNextTheme()
        {
            int currentThemeIndex = themeManager.CurrentThemeIndex;

            if (currentThemeIndex >= themeManager.AllThemes.Count - 1) return;

            Theme themeToBeSelected = themeManager.AllThemes[currentThemeIndex + 1];

            themeManager.SelectTheme(themeToBeSelected);
            SetCurrentThemeUI(themeToBeSelected);
        }

        public void SetPreviousTheme()
        {
            int currentThemeIndex = themeManager.CurrentThemeIndex;

            if (currentThemeIndex <= 0) return;

            Theme themeToBeSelected = themeManager.AllThemes[currentThemeIndex - 1];

            themeManager.SelectTheme(themeToBeSelected);
            SetCurrentThemeUI(themeToBeSelected);
        }

        private void SetCurrentThemeUI(Theme themeToBeSelected)
        {
            Theme selectedTheme = themeToBeSelected;

            themeText.text = selectedTheme.name;
            themeImage.sprite = selectedTheme.BackgroundImage;
        }
    }
}