using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.UI;

namespace Crush2048
{
    [CreateAssetMenu(fileName = "TileType", menuName = "ScriptableObjects/TileType")]
    public class TileType : ScriptableObject
    {
        public int pointsWorth;
        public bool isSpecial;
        public Sprite image;
        public Behaviours tileBehaviour;

        private const int MINIMUM_POINTS_WORTH = 0;

        private void OnValidate() => ClampPointsWorth();

        private void ClampPointsWorth() => pointsWorth = pointsWorth % 2 == 0 ? Mathf.Max(MINIMUM_POINTS_WORTH, pointsWorth) : MINIMUM_POINTS_WORTH;


#if UNITY_EDITOR
        [CustomEditor(typeof(TileType))]
        class TileTypeEditor : Editor
        {
            SerializedProperty _isSpecial;
            SerializedProperty _image;
            SerializedProperty _pointsWorth;
            SerializedProperty _tileBehaviour;

            private void OnEnable()
            {
                _isSpecial = serializedObject.FindProperty("isSpecial");
                _image = serializedObject.FindProperty("image");
                _pointsWorth = serializedObject.FindProperty("pointsWorth");
                _tileBehaviour = serializedObject.FindProperty("tileBehaviour");
            }

            public override void OnInspectorGUI()
            {
                serializedObject.Update();

                EditorGUILayout.PropertyField(_isSpecial);

                if(_isSpecial.boolValue)
                {
                    EditorGUILayout.PropertyField(_image);
                    EditorGUILayout.PropertyField(_tileBehaviour);
                }
                else
                {
                    EditorGUILayout.PropertyField(_pointsWorth);
                    _tileBehaviour.enumValueIndex = (int)Behaviours.Default;
                }

                serializedObject.ApplyModifiedProperties();
            }
        }
#endif
    }
}
