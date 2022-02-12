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

        private void Start() => SetCurrentThemeUI();

        public void SetNextTheme()
        {
            int currentThemeIndex = themeManager.CurrentThemeIndex;

            if (currentThemeIndex >= themeManager.AllThemes.Count - 1) return;

            Theme themeToBeSelected = themeManager.AllThemes[currentThemeIndex + 1];

            themeManager.SelectTheme(themeToBeSelected);
            SetCurrentThemeUI();
        }

        public void SetPreviousTheme()
        {
            int currentThemeIndex = themeManager.CurrentThemeIndex;

            if (currentThemeIndex <= 0) return;

            Theme themeToBeSelected = themeManager.AllThemes[currentThemeIndex - 1];

            themeManager.SelectTheme(themeToBeSelected);
            SetCurrentThemeUI();
        }

        private void SetCurrentThemeUI()
        {
            Theme selectedTheme = themeManager.ThemeSelected;

            themeText.text = selectedTheme.name;
            themeImage.sprite = selectedTheme.BackgroundImage;
        }
    }
}
