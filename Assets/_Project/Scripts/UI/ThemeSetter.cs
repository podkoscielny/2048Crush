using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using DG.Tweening;

namespace Crush2048
{
    public class ThemeSetter : MonoBehaviour
    {
        [SerializeField] ThemeManager themeManager;
        [SerializeField] Image themeImage;
        [SerializeField] TextMeshProUGUI themeText;

        private Vector3 _initialTextScale;
        private Sequence _punchScaleSequence;

        private const float ANIMATION_DURATION = 0.15f;
        private const float ENLARGED_FACTOR = 1.4f;

        private void Awake() => _initialTextScale = themeText.rectTransform.localScale;

        private void Start() => SetCurrentThemeUI(themeManager.ThemeSelected);

        public void SetNextTheme()
        {
            int currentThemeIndex = themeManager.CurrentThemeIndex;

            if (currentThemeIndex >= themeManager.AllThemes.Count - 1) return;

            Theme themeToBeSelected = themeManager.AllThemes[currentThemeIndex + 1];

            themeManager.SelectTheme(themeToBeSelected);
            SetCurrentThemeUI(themeToBeSelected);
            PunchScaleText();
        }

        public void SetPreviousTheme()
        {
            int currentThemeIndex = themeManager.CurrentThemeIndex;

            if (currentThemeIndex <= 0) return;

            Theme themeToBeSelected = themeManager.AllThemes[currentThemeIndex - 1];

            themeManager.SelectTheme(themeToBeSelected);
            SetCurrentThemeUI(themeToBeSelected);
            PunchScaleText();
        }

        private void SetCurrentThemeUI(Theme themeToBeSelected)
        {
            Theme selectedTheme = themeToBeSelected;

            themeText.text = selectedTheme.name;
            themeImage.sprite = selectedTheme.BackgroundImage;
        }

        private void PunchScaleText()
        {
            _punchScaleSequence = DOTween.Sequence().SetAutoKill(false);

            _punchScaleSequence.Append(themeText.rectTransform.DOScale(_initialTextScale * ENLARGED_FACTOR, ANIMATION_DURATION).SetEase(Ease.OutCirc));
            _punchScaleSequence.Append(themeText.rectTransform.DOScale(_initialTextScale, ANIMATION_DURATION).SetEase(Ease.OutCirc));
        }
    }
}
