using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

namespace TileMapperEditor
{
    [CustomEditor(typeof(TileMapper.Controller))]
    public class ControllerEditor : Editor
    {
        public ControllerEditor()
        {

        }

        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();

            EditorGUILayout.BeginHorizontal();
            {
                EditorGUILayout.LabelField("Self", GUILayout.Width(115));
                TileMapper.Controller.Self = EditorGUILayout.ObjectField(TileMapper.Controller.Self, typeof(TileMapper.Controller), true) as TileMapper.Controller;
            }
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Sorting Layers", EditorStyles.boldLabel);
            foreach (string Layer in TileMapper.Controller.SortingLayers)
                EditorGUILayout.LabelField(Layer);
        }
    }
}