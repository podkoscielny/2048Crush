using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Crush2048
{
    [CreateAssetMenu(fileName = "ThemeManager", menuName = "ScriptableObjects/ThemeManager")]
    public class ThemeManager : ScriptableObject
    {
        public static event Action OnThemeChanged;

        [Header("Theme")]
        [SerializeField] Theme themeSelected;
        [SerializeField] List<Theme> allThemes;

        [Header("Materials")]
        [SerializeField] Material primaryMaterial;
        [SerializeField] Material secondaryMaterial;
        [SerializeField] Material UIPrimaryMaterial;
        [SerializeField] Material UISecondaryMaterial;
        [SerializeField] Material[] textMaterials;

        public Theme ThemeSelected => themeSelected;
        public List<Theme> AllThemes => allThemes;
        public int CurrentThemeIndex => allThemes.IndexOf(themeSelected);

        private List<Material> _allThemeMaterials = new List<Material>();

        private const string SAVE_PATH = "theme";


        private void OnValidate()
        {
            SaveTheme();
            AssignMaterialsToList();
            ChangeThemeColors();
        }

        private void OnEnable() => LoadTheme();

        public void SelectTheme(Theme theme)
        {
            themeSelected = theme;

            SaveTheme();
            ChangeThemeColors();
        }

        private void SaveTheme() => SaveSystem.Save<string>(SAVE_PATH, themeSelected.name);

        private void LoadTheme()
        {
            string themeName = SaveSystem.Load<string>(SAVE_PATH);

            if (allThemes.Count > 0)
            {
                foreach (Theme theme in allThemes)
                {
                    if (themeName == theme.name)
                    {
                        themeSelected = theme;
                        break;
                    }
                }

                if (themeSelected == null) themeSelected = allThemes[0];

                ChangeThemeColors();
            }
        }

        private void ChangeThemeColors()
        {
            if (CantChangeColors()) return;

            primaryMaterial.color = themeSelected.PrimaryColor;
            secondaryMaterial.color = themeSelected.SecondaryColor;
            UIPrimaryMaterial.color = themeSelected.PrimaryColor;
            UISecondaryMaterial.color = themeSelected.SecondaryColor;

            foreach (var material in textMaterials)
            {
                material.SetColor("_FaceColor", themeSelected.TextColor);
            }

            OnThemeChanged?.Invoke();
        }

        private bool CantChangeColors() => themeSelected == null || textMaterials.Length < 1 || ArentAllMaterialsAssigned();

        private bool ArentAllMaterialsAssigned()
        {
            if (_allThemeMaterials.Count <= 0) AssignMaterialsToList();

            return _allThemeMaterials.Contains(null);
        }

        private void AssignMaterialsToList()
        {
            _allThemeMaterials.Add(primaryMaterial);
            _allThemeMaterials.Add(secondaryMaterial);
            _allThemeMaterials.Add(UIPrimaryMaterial);
            _allThemeMaterials.Add(UISecondaryMaterial);
        }
    }
}
