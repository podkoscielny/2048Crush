using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Crush2048
{
    [CreateAssetMenu(fileName = "TileGradients", menuName = "ScriptableObjects/TileGradients")]
    public class TileGradients : ScriptableObject
    {
        [SerializeField] Gradient background;
        [SerializeField] Gradient text;

        public Color RecentlyPickedBackgroundColor { get; private set; } = Color.white;

        public Color GetBackgroundColor(float gradientPercentage)
        {
            Color pickedColor = background.Evaluate(gradientPercentage);

            RecentlyPickedBackgroundColor = pickedColor;

            return pickedColor;
        }

        public Color GetTextColor(float gradientPercentage) => text.Evaluate(gradientPercentage);
    }
}