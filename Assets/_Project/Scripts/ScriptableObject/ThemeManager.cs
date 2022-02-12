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

        private List<Material> _allThemeMaterials;

        public Theme ThemeSelected { get => themeSelected; private set => themeSelected = value; }

        private void OnValidate()
        {
            AssignMaterialsToList();
            ChangeThemeColors();
        }

        public void SelectTheme(Theme theme)
        {
            ThemeSelected = theme;

            ChangeThemeColors();
        }

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

        private bool CantChangeColors() => themeSelected == null || textMaterials.Length < 1 || ArentAllMaterialsAssigned();

        private bool ArentAllMaterialsAssigned()
        {
            if (_allThemeMaterials.Count < 1) AssignMaterialsToList();

            return _allThemeMaterials.Contains(null);
        }

        private void AssignMaterialsToList()
        {
            _allThemeMaterials = new List<Material>();

            _allThemeMaterials.Add(primaryMaterial);
            _allThemeMaterials.Add(secondaryMaterial);
            _allThemeMaterials.Add(UIPrimaryMaterial);
            _allThemeMaterials.Add(UISecondaryMaterial);
        }
    }
}
