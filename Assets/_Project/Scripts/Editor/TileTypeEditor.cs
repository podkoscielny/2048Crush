using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

#if UNITY_EDITOR
namespace Crush2048
{
    [CustomEditor(typeof(TileType))]
    public class TileTypeEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            DrawProperty("isSpecial");

            DrawConditionalProperties();

            serializedObject.ApplyModifiedProperties();
        }

        private void DrawConditionalProperties()
        {
            if (GetProperty("isSpecial").boolValue)
            {
                DrawProperty("behaviour");
                DrawProperty("showTextInsteadOfImage");

                if (!GetProperty("showTextInsteadOfImage").boolValue) DrawProperty("image");

                if (GetProperty("showTextInsteadOfImage").boolValue) DrawProperty("textToShow");
            }
            else
                DrawProperty("pointsWorth");
        }

        private void DrawProperty(string propertyPath) => EditorGUILayout.PropertyField(GetProperty(propertyPath));

        private SerializedProperty GetProperty(string propertyPath) => serializedObject.FindProperty(propertyPath);
    }
}
#endif
