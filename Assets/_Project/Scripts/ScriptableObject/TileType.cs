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

    public int PointsWorth => pointsWorth;

    private const int MINIMUM_POINTS_WORTH = 0;

    private void OnValidate() => ClampPointsWorth();

    private void ClampPointsWorth() => pointsWorth = pointsWorth % 2 == 0 ? Mathf.Max(MINIMUM_POINTS_WORTH, pointsWorth) : MINIMUM_POINTS_WORTH;
}

#if UNITY_EDITOR
[CustomEditor(typeof(TileType))]
class TileTypeEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        
    }
}
#endif
