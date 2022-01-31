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

        [Header("Materials")]
        [SerializeField] Material primaryMaterial;
        [SerializeField] Material secondaryMaterial;
        [SerializeField] Material UIPrimaryMaterial;
        [SerializeField] Material UISecondaryMaterial;
        [SerializeField] Material[] textMaterials;

        public Theme ThemeSelected { get => themeSelected; private set => themeSelected = value; }

        private void OnValidate() => ChangeThemeColors();

        public void SelectTheme(Theme theme)
        {
            ThemeSelected = theme;

            ChangeThemeColors();
        }

        private bool CantChangeColors() => themeSelected == null || textMaterials.Length < 1 || primaryMaterial == null || secondaryMaterial == null || UIPrimaryMaterial == null || UISecondaryMaterial == null;

        private void ChangeThemeColors()
        {
            if (CantChangeColors()) return;

            OnThemeChanged?.Invoke();

            primaryMaterial.color = themeSelected.PrimaryColor;
            secondaryMaterial.color = themeSelected.SecondaryColor;
            UIPrimaryMaterial.color = themeSelected.PrimaryColor;
            UISecondaryMaterial.color = themeSelected.SecondaryColor;

            foreach (var material in textMaterials)
            {
                material.SetColor("_FaceColor", themeSelected.TextColor);
            }
        }
    }
}
