using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace TileMapperEditor
{
    public class ControllerWindow : EditorWindow
    {
        private Texture2D[,][] m_TextureCache;
        private Texture2D m_SelectorTexture;
        private Texture2D m_CursorDrag;

        private Vector2 m_ScrollPos;

        private Vector2 m_MouseGridPosition, m_OldMouseGridPosition;

        private Vector2 m_SelectorPosition, m_OldSelectorPosition, m_SelectorGridPosition;

        private Vector2 m_OldMousePosition;
        private bool m_ScrollFollowMouse;

        private Vector2 m_ImageSize = new Vector2(32.0f, 32.0f);
        private Vector2 m_ImageOffset = new Vector2(30.0f, 30.0f);

        private Vector2 m_GridViewOffset = new Vector2(0.0f, 100.0f);

        private GUIStyle m_CenteredLabel;

        [DllImport("user32.dll")]
        static public extern bool SetCursorPos(int X, int Y);

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

            m_MouseGridPosition = Vector2.zero;
            m_OldMouseGridPosition = m_MouseGridPosition;

            m_SelectorGridPosition = Vector2.zero;
            m_SelectorPosition = new Vector2(
                (m_SelectorGridPosition.x * m_ImageSize.x) + m_ImageOffset.x,
                (m_SelectorGridPosition.y * m_ImageSize.y) + m_ImageOffset.y);
            m_OldSelectorPosition = m_SelectorPosition;

            m_ScrollFollowMouse = false;
        }

        void OnGUI()
        {
            if (m_TextureCache == null)
                CacheTextures();

            if (m_CenteredLabel == null)
            {
                m_CenteredLabel = GUI.skin.GetStyle("Label");
                m_CenteredLabel.alignment = TextAnchor.MiddleCenter;
            }

            m_MouseGridPosition = GUIUtility.GUIToScreenPoint(Event.current.mousePosition);
            m_MouseGridPosition = new Vector2(
                m_MouseGridPosition.x - position.x - m_ImageOffset.x - (m_ImageSize.x / 2.0f) - m_GridViewOffset.x + m_ScrollPos.x,
                m_MouseGridPosition.y - position.y - m_ImageOffset.y - (m_ImageSize.x / 2.0f) - m_GridViewOffset.y + m_ScrollPos.y);

            m_MouseGridPosition = TileMapper.Controller.GetGridPosition(m_MouseGridPosition, new Vector3(m_ImageSize.x, m_ImageSize.y), Vector3.zero);

            if (m_MouseGridPosition != m_OldMouseGridPosition)
            {
                m_OldMouseGridPosition = m_MouseGridPosition;
                //Repaint();
            }

            Event E = Event.current;
            switch (E.type)
            {
                case EventType.MouseUp:
                    {
                        if (E.button == 0)
                        {
                            if (m_MouseGridPosition.x >= 0 && m_MouseGridPosition.x < m_TextureCache.GetLength(0) &&
                                m_MouseGridPosition.y >= 0 && m_MouseGridPosition.y < m_TextureCache.GetLength(1))
                            {
                                m_SelectorGridPosition = m_MouseGridPosition;
                                m_SelectorPosition = new Vector2(
                                    (m_SelectorGridPosition.x * m_ImageSize.x) + m_ImageOffset.x,
                                    (m_SelectorGridPosition.y * m_ImageSize.y) + m_ImageOffset.y);
                            }
                        }
                        else if (E.button == 1)
                        {
                            Cursor.visible = true;
                            m_ScrollFollowMouse = false;
                        }
                    }
                    break;
                case EventType.MouseDown:
                    {
                        if (E.button == 1)
                        {
                            if (m_MouseGridPosition.x >= 0 && m_MouseGridPosition.x <= m_TextureCache.GetLength(0) &&
                                m_MouseGridPosition.y >= 0 && m_MouseGridPosition.y <= m_TextureCache.GetLength(1))
                            {
                                Cursor.visible = false;
                                m_OldMousePosition = GUIUtility.GUIToScreenPoint(Event.current.mousePosition);
                                m_ScrollFollowMouse = true;
                            }
                        }
                    }
                    break;
            }

            if (m_OldSelectorPosition != m_SelectorPosition)
            {
                m_OldSelectorPosition = m_SelectorPosition;
                Repaint();
            }

            EditorGUILayout.LabelField(m_SelectorGridPosition.ToString());
            for (int i = 0; i < m_TextureCache[(int)m_SelectorGridPosition.x, (int)m_SelectorGridPosition.y].Length; ++i)
            {
                GUI.DrawTexture(new Rect(75 + (i * 16), 0, 16, 16), m_TextureCache[(int)m_SelectorGridPosition.x, (int)m_SelectorGridPosition.y][i]);
            }

            m_ImageSize = EditorGUILayout.Vector2Field("Image Size", m_ImageSize, GUILayout.Width(150));
            m_ImageSize = new Vector2(Mathf.Ceil(Mathf.Clamp(m_ImageSize.x, 1, m_ImageSize.x)), Mathf.Ceil(Mathf.Clamp(m_ImageSize.y, 1, m_ImageSize.y)));

            if (GUILayout.Button("Refresh", GUILayout.Width(75)))
            {
                TileMapper.Controller.CreateArray();
                CacheTextures();
            }

            m_ScrollPos = GUI.BeginScrollView(
                new Rect(m_GridViewOffset.x, m_GridViewOffset.y, Screen.width, Screen.height - m_GridViewOffset.y - 20), m_ScrollPos,
                new Rect(0, 0, (m_ImageSize.x * (m_TextureCache.GetLength(0) + 1)) + m_ImageOffset.x, (m_ImageSize.y * (m_TextureCache.GetLength(1) + 1)) + m_ImageOffset.y));
            {
                for (int i = 0; i < m_TextureCache.GetLength(0); ++i)
                {
                    EditorGUI.LabelField(new Rect(m_ImageOffset.x + (i * m_ImageSize.x), 0, m_ImageSize.x, m_ImageSize.y), i.ToString(), m_CenteredLabel);
                    for (int j = 0; j < m_TextureCache.GetLength(1); ++j)
                    {
                        if (i == 0)
                            EditorGUI.LabelField(new Rect(0, m_ImageOffset.y + (j * m_ImageSize.y), m_ImageSize.x, m_ImageSize.y), j.ToString(), m_CenteredLabel);
                        for (int k = 0; k < m_TextureCache[i, j].Length; ++k)
                        {
                            if (m_TextureCache[i, j][k] == null)
                                continue;

                            GUI.DrawTexture(new Rect(m_ImageOffset.x + (i * m_ImageSize.x), m_ImageOffset.y + (j * m_ImageSize.y), m_ImageSize.x, m_ImageSize.y), m_TextureCache[i, j][k]);
                        }
                    }
                }
                GUI.DrawTexture(new Rect(m_SelectorPosition.x, m_SelectorPosition.y, m_ImageSize.x, m_ImageSize.y), m_SelectorTexture);
            }
            GUI.EndScrollView();

            if (m_ScrollFollowMouse)
            {
                Vector2 CurrentMousePosition = GUIUtility.GUIToScreenPoint(Event.current.mousePosition);

                m_ScrollPos -= CurrentMousePosition - m_OldMousePosition;

                GUI.DrawTexture(new Rect(m_OldMousePosition.x - position.x - 16, m_OldMousePosition.y - position.y - 16, 32, 32), m_CursorDrag);

                if (CurrentMousePosition.x <= position.x + m_GridViewOffset.x)
                {
                    int NewMousePosition = (int)(position.x + m_GridViewOffset.x);

                    SetCursorPos(NewMousePosition, (int)CurrentMousePosition.y);
                    CurrentMousePosition.x = NewMousePosition;
                }
                else if (CurrentMousePosition.x >= position.x + Screen.width - 15)
                {
                    int NewMousePosition = (int)(position.x + +Screen.width - 15);

                    SetCursorPos(NewMousePosition, (int)CurrentMousePosition.y);
                    CurrentMousePosition.x = NewMousePosition;
                }

                if (CurrentMousePosition.y <= position.y + m_GridViewOffset.y)
                {
                    int NewMousePosition = (int)(position.y + m_GridViewOffset.y);

                    SetCursorPos((int)CurrentMousePosition.x, NewMousePosition);
                    CurrentMousePosition.y = NewMousePosition;
                }
                else if (CurrentMousePosition.y >= position.y + Screen.height - 35)
                {
                    int NewMousePosition = (int)(position.y + Screen.height - 35);

                    SetCursorPos((int)CurrentMousePosition.x, NewMousePosition);
                    CurrentMousePosition.y = NewMousePosition;
                }

                m_OldMousePosition = CurrentMousePosition;

                Repaint();
            }
        }

        void CacheTextures()
        {
            if (TileMapper.Controller.Tiles != null)
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

            m_SelectorTexture = Resources.Load<Texture2D>("Selector");
            m_SelectorTexture.filterMode = FilterMode.Point;

            m_CursorDrag = Resources.Load<Texture2D>("Cursor_Drag");
            m_CursorDrag.filterMode = FilterMode.Point;
        }
    }
}
