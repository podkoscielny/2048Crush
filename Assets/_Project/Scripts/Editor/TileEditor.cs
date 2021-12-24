using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

#if UNITY_EDITOR
[CustomEditor(typeof(Tile))]
public class TileEditor : Editor
{
    int pointsToSet = 2;

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        DrawSetPointsUI();
    }

    private void DrawSetPointsUI()
    {
        Tile tile = (Tile)target;

        GUILayout.BeginHorizontal();

        pointsToSet = EditorGUILayout.DelayedIntField("Points To Set", pointsToSet);

        if (GUILayout.Button("Set Points")) tile.ChangeTilePointsWorth(pointsToSet);

        GUILayout.EndHorizontal();
    }
}
#endif