using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using Tags = MultipleTagSystem.TagSystem.Tags;

namespace Crush2048
{
    [CreateAssetMenu(fileName = "TileType", menuName = "ScriptableObjects/TileType")]
    public class TileType : ScriptableObject
    {
        public int pointsWorth;
        public bool isSpecial;

        //Special Behaviour fields
        public Sprite image;
        public BaseBehaviour behaviour;

        private const int MINIMUM_POINTS_WORTH = 0;

        private void OnValidate() => ClampPointsWorth();

        private void ClampPointsWorth() => pointsWorth = pointsWorth % 2 == 0 ? Mathf.Max(MINIMUM_POINTS_WORTH, pointsWorth) : MINIMUM_POINTS_WORTH;
    }
}
