using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

namespace Crush2048
{
    public class ThemeSetter : MonoBehaviour
    {
        [SerializeField] ThemeManager themeManager;
        [SerializeField] TextMeshProUGUI themeText;

        private void Start() => SetCurrentThemeName();

        public void SetNextTheme()
        {
            int currentThemeIndex = themeManager.CurrentThemeIndex;

            if (currentThemeIndex >= themeManager.AllThemes.Count - 1) return;

            Theme themeToBeSelected = themeManager.AllThemes[currentThemeIndex + 1];

            themeManager.SelectTheme(themeToBeSelected);
            SetCurrentThemeName();
        }

        public void SetPreviousTheme()
        {
            int currentThemeIndex = themeManager.CurrentThemeIndex;

            if (currentThemeIndex <= 0) return;

            Theme themeToBeSelected = themeManager.AllThemes[currentThemeIndex - 1];

            themeManager.SelectTheme(themeToBeSelected);
            SetCurrentThemeName();
        }

        private void SetCurrentThemeName() => themeText.text = themeManager.ThemeSelected.name;
    }
}
