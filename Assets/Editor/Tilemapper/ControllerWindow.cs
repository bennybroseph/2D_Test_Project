using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

namespace TileMapperEditor
{
    public class ControllerWindow : EditorWindow
    {
        private bool[] m_ShowBaseFoldouts;
        private bool[,] m_ShowFoldouts;

        // Add menu named "My Window" to the Window menu
        [MenuItem("Window/My Window")]
        static void Init()
        {
            // Get existing open window or if none, make a new one:
            ControllerWindow Window = (ControllerWindow)GetWindow(typeof(ControllerWindow));
            Window.Show();
        }

        public ControllerWindow()
        {
            m_ShowFoldouts = new bool[TileMapper.Controller.Tiles.GetLength(0), TileMapper.Controller.Tiles.GetLength(1)];
            m_ShowBaseFoldouts = new bool[TileMapper.Controller.Tiles.GetLength(0)];
        }

        void OnGUI()
        {
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