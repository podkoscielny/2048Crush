using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

#if UNITY_EDITOR
[CustomEditor(typeof(TilePoints))]
public class TilePointsEditor : Editor
{
    private int pointsToSet = 2;

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        DrawSetPointsUI();
    }

    private void DrawSetPointsUI()
    {
        TilePoints tilePoints = (TilePoints)target;

        SetTilePoints(tilePoints);
        RandomizeAllTilePoints();
    }

    private void SetTilePoints(TilePoints tilePoints)
    {
        GUILayout.BeginHorizontal();

        pointsToSet = EditorGUILayout.DelayedIntField("Points To Set", pointsToSet);

        if (GUILayout.Button("Set Points")) tilePoints.ChangeTilePointsWorth(pointsToSet);

        GUILayout.EndHorizontal();
    }

    private void RandomizeAllTilePoints()
    {
        GUILayout.BeginHorizontal();


        if (GUILayout.Button("Randomize Points"))
        {
            TilePoints[] tiles = FindObjectsOfType<TilePoints>();

            foreach (TilePoints tile in tiles)
            {
                tile.RandomizeTilePoints();
            }
        }

        GUILayout.EndHorizontal();
    }
}
#endif
