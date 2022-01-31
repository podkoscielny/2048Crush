using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Crush2048
{
    [CreateAssetMenu(fileName = "Theme", menuName = "ScriptableObjects/Theme")]
    public class Theme : ScriptableObject
    {
        [SerializeField] Color textColor;
        [SerializeField] Color primaryColor;
        [SerializeField] Color secondaryColor;
        [SerializeField] Color backgroundColor;
        [SerializeField] Sprite backgroundImage;

        public Color TextColor => textColor;
        public Color PrimaryColor => primaryColor;
        public Color SecondaryColor => secondaryColor;
        public Color BackgroundColor => backgroundColor;
        public Sprite BackgroundImage => backgroundImage;
    }
}
