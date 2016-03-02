using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

namespace TileMapperEditor
{
    [CustomEditor(typeof(TileMapper.Controller))]
    public class LevelScriptEditor : Editor
    {
        public bool[] m_ShowBaseFoldouts;
        public bool[,] m_ShowFoldouts;

        public LevelScriptEditor()
        {
            m_ShowFoldouts = new bool[TileMapper.Controller.Tiles.GetLength(0), TileMapper.Controller.Tiles.GetLength(1)];
            m_ShowBaseFoldouts = new bool[TileMapper.Controller.Tiles.GetLength(0)];
        }

        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();

            EditorGUILayout.BeginHorizontal();

            EditorGUILayout.LabelField("Self", GUILayout.Width(115));
            TileMapper.Controller.Self = EditorGUILayout.ObjectField(TileMapper.Controller.Self, typeof(TileMapper.Controller), true) as TileMapper.Controller;

            EditorGUILayout.EndHorizontal();

            for (int i = 0; i < TileMapper.Controller.Tiles.GetLength(0); ++i)
                if (m_ShowBaseFoldouts[i] = EditorGUILayout.Foldout(m_ShowBaseFoldouts[i], i.ToString()))
                {
                    EditorGUI.indentLevel++;
                    for (int j = 0; j < TileMapper.Controller.Tiles.GetLength(1); ++j)
                        if (m_ShowFoldouts[i, j] = EditorGUILayout.Foldout(m_ShowFoldouts[i, j], i.ToString() + ", " + j.ToString()))
                        {
                            EditorGUI.indentLevel++;
                            int ListIndex = 0;
                            foreach (GameObject gameObject in TileMapper.Controller.Tiles[i, j])
                            {
                                EditorGUILayout.BeginHorizontal();
                                EditorGUILayout.LabelField(ListIndex.ToString(), GUILayout.Width(115));
                                EditorGUILayout.ObjectField(gameObject, typeof(GameObject), true);
                                EditorGUILayout.EndHorizontal();
                                ListIndex++;
                            }
                            EditorGUI.indentLevel--;
                        }
                    EditorGUI.indentLevel--;
                }
        }
    }
}