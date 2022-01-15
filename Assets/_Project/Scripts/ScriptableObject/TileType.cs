using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.UI;

[CreateAssetMenu(fileName = "TileType", menuName = "ScriptableObjects/TileType")]
public class TileType : ScriptableObject
{
    [SerializeField] int pointsWorth;
    [SerializeField] bool isSpecial;
    [SerializeField] Image image;
    [SerializeField] TileBehaviour tileBehaviour;

    public int PointsWorth => pointsWorth;

    private const int MINIMUM_POINTS_WORTH = 0;

    private void OnValidate() => ClampPointsWorth();

    private void ClampPointsWorth() => pointsWorth = pointsWorth % 2 == 0 ? Mathf.Max(MINIMUM_POINTS_WORTH, pointsWorth) : MINIMUM_POINTS_WORTH;




#if UNITY_EDITOR
    [CustomEditor(typeof(TileType))]
    class TileTypeEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            TileType tileType = (TileType)target;

            tileType.isSpecial = EditorGUILayout.Toggle("Is Special", tileType.isSpecial);

            if (tileType.isSpecial)
            {
                tileType.image = (Image)EditorGUILayout.ObjectField("Image", tileType.image, typeof(Image), false);
                tileType.tileBehaviour = (TileBehaviour)EditorGUILayout.EnumPopup("Tile Behaviour", tileType.tileBehaviour);
            }
            else
            {
                tileType.pointsWorth = EditorGUILayout.DelayedIntField("Points Worth", tileType.pointsWorth);
                tileType.tileBehaviour = TileBehaviour.Default;
            }


        }
    }
#endif
}

public enum TileBehaviour
{
    Default,
    MultiplyAnyTile,
    MatchAnyTile,
    BombNearbyTiles
}
