using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

namespace TileMapperEditor
{
    [CustomEditor(typeof(TileMapper.Controller))]
    public class LevelScriptEditor : Editor
    {
        public bool[,] ShowFoldouts;

        public LevelScriptEditor()
        {
            ShowFoldouts = new bool[TileMapper.Controller.Tiles.GetLength(0), TileMapper.Controller.Tiles.GetLength(1)];
        }

        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();            

            EditorGUILayout.BeginHorizontal();

            EditorGUILayout.LabelField("Self", GUILayout.Width(115));
            TileMapper.Controller.Self = EditorGUILayout.ObjectField(TileMapper.Controller.Self, typeof(TileMapper.Controller), true) as TileMapper.Controller;

            EditorGUILayout.EndHorizontal();            

            for (int i = 0; i < TileMapper.Controller.Tiles.GetLength(0); ++i)
                for (int j = 0; j < TileMapper.Controller.Tiles.GetLength(1); ++j)
                    if (ShowFoldouts[i, j] = EditorGUILayout.Foldout(ShowFoldouts[i, j], i.ToString() + ", " + j.ToString()))
                    {
                        foreach (GameObject gameObject in TileMapper.Controller.Tiles[i, j])
                        {
                            EditorGUILayout.BeginHorizontal();
                            EditorGUILayout.LabelField(i.ToString() + ", " + j.ToString(), GUILayout.Width(115));
                            EditorGUILayout.ObjectField(gameObject, typeof(GameObject), true);
                            EditorGUILayout.EndHorizontal();
                        }
                    }
        }
    }
}