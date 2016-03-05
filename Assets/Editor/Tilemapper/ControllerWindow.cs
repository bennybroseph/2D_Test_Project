using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

namespace TileMapperEditor
{
    public class ControllerWindow : EditorWindow
    {
        private Texture2D[,][] m_TextureCache;
        private Vector2 m_ScrollPos;

        private Vector2 m_MousePosition, m_OldMousePosition;

        private Vector2 m_ImageScale = new Vector2(2.0f, 2.0f);

        // Add menu named "My Window" to the Window menu
        [MenuItem("TileMapper/My Window")]
        static void Init()
        {
            // Get existing open window or if none, make a new one:
            ControllerWindow Window = (ControllerWindow)GetWindow(typeof(ControllerWindow));
            Window.Show();
        }

        public ControllerWindow()
        {
            wantsMouseMove = true;

            CacheTextures();

            m_MousePosition = new Vector2();
            m_OldMousePosition = m_MousePosition;
        }

        void Update()
        {
            if (EditorApplication.isCompiling)
                Close();
        }
        void OnGUI()
        {
            m_MousePosition = GUIUtility.GUIToScreenPoint(Event.current.mousePosition);
            m_MousePosition = new Vector2(m_MousePosition.x - position.x + m_ScrollPos.x, m_MousePosition.y - position.y + m_ScrollPos.y);
            m_MousePosition = TileMapper.Controller.GetGridPosition(m_MousePosition, new Vector3(32, 32, 0), Vector3.zero);

            if (m_MousePosition != m_OldMousePosition)
            {
                Repaint();
                m_OldMousePosition = m_MousePosition;
            }

            EditorGUILayout.LabelField(m_MousePosition.ToString());
            if (GUILayout.Button("Refresh", GUILayout.Width(75)))
            {
                TileMapper.Controller.CreateArray();
                CacheTextures();
            }

            m_ScrollPos = GUI.BeginScrollView(new Rect(0, 100, Screen.width, Screen.height - 125), m_ScrollPos, new Rect(0, 0, 1000, 1000));
            for (int i = 0; i < m_TextureCache.GetLength(0); ++i)
            {

                EditorGUI.LabelField(new Rect(35 + (i * 32) - (i.ToString().Length * 5), 0, 32, 32), i.ToString());
                for (int j = 0; j < m_TextureCache.GetLength(1); ++j)
                {
                    if (i == 0)
                        EditorGUI.LabelField(new Rect(10 - (j.ToString().Length * 5), 25 + (j * 32), 32, 32), j.ToString());
                    for (int k = 0; k < m_TextureCache[i, j].Length; ++k)
                    {
                        if (m_TextureCache[i, j][k] == null)
                            continue;

                        GUI.DrawTexture(new Rect(20 + (i * 32), 20 + (j * 32), 32, 32), m_TextureCache[i, j][k]);
                    }
                }
            }
            GUI.EndScrollView();
        }

        void CacheTextures()
        {
            m_TextureCache = new Texture2D[TileMapper.Controller.Tiles.GetLength(0), TileMapper.Controller.Tiles.GetLength(1)][];

            for (int i = 0; i < m_TextureCache.GetLength(0); ++i)
                for (int j = 0; j < m_TextureCache.GetLength(1); ++j)
                {
                    m_TextureCache[i, j] = new Texture2D[TileMapper.Controller.Tiles[i, j].Count];
                    for (int k = 0; k < m_TextureCache[i, j].Length; ++k)
                    {
                        Texture2D TextureToCache;

                        if (TileMapper.Controller.Tiles[i, j][k] == null)
                            TextureToCache = Resources.Load<Texture2D>("NULL_SPRITE");
                        else
                        {
                            Rect SpriteRect = TileMapper.Controller.Tiles[i, j][k].GetComponent<SpriteRenderer>().sprite.rect;
                            TextureToCache = new Texture2D((int)SpriteRect.width, (int)SpriteRect.height);
                            Color[] PixelData = TileMapper.Controller.Tiles[i, j][k].GetComponent<SpriteRenderer>().sprite.texture.GetPixels((int)SpriteRect.x, (int)SpriteRect.y, (int)SpriteRect.width, (int)SpriteRect.height);
                            TextureToCache.SetPixels(PixelData);
                            TextureToCache.Apply(true);
                        }
                        TextureToCache.filterMode = FilterMode.Point;


                        m_TextureCache[i, j][k] = TextureToCache;
                    }
                }
        }
    }
}
