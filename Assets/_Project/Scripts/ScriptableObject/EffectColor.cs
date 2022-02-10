using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Crush2048
{
    [CreateAssetMenu(fileName = "EffectColor", menuName = "ScriptableObjects/EffectColor")]
    public class EffectColor : ScriptableObject
    {
        [SerializeField] Color color;
        [SerializeField] Material effectMaterial;

        public Color Color => color;

        public void SetColor(Color newColor)
        {
            effectMaterial.color = newColor;
            effectMaterial.SetColor("_EmissionColor", newColor);
        }
    }
}