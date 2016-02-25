using UnityEngine;
using UnityEditor;
using System;
using System.Linq;
using System.Collections.Generic;

namespace TileMapper
{
    [ExecuteInEditMode]
    class Wall : Tile
    {
        public enum Orientation { TOP, BOTTOM, LEFT, RIGHT, CENTER };

        [Serializable]
        public struct Position
        {
            public Orientation Vertical;
            public Orientation Horizontal;

            public Position(Orientation a_Vertical, Orientation a_Horizontal)
            {
                Vertical = a_Vertical;
                Horizontal = a_Horizontal;
            }
        }

        [SerializeField]
        protected GameObject m_HorizontalFillPrefab;
        [SerializeField]
        protected GameObject m_BottomEdgePrefab;
        [SerializeField]
        protected GameObject m_VerticalFillPrefab;
        [SerializeField]
        protected GameObject m_TopEdgePrefab;

        [SerializeField]
        protected Dictionary<Orientation, Wall> m_Touching;
        [SerializeField]
        protected Orientation m_VerticalPosition;
        [SerializeField]
        protected Orientation m_HorizontalPosition;

        List<Wall> m_OtherWalls = new List<Wall>();

        // The wall objects which have already connected to this one
        protected List<Wall> m_CurrentlyConnected = new List<Wall>();
        protected SpriteRenderer m_SpriteRenderer;

        // Used to instantiate the requred objects to fill the space
        public void Update()
        {
            // Checks to make sure the user didn't forget to add a sprite renderer
            // One is not required, but recommended for error checking
            if ((m_SpriteRenderer = GetComponent<SpriteRenderer>()) == null)
                Debug.LogWarning(name + " has no sprite renderer!");

            // Collect all children, then delete them
            List<GameObject> Children = new List<GameObject>();
            foreach (Transform child in transform)
                Children.Add(child.gameObject);
            Children.ForEach(child => DestroyImmediate(child));

            SnapToGrid();

            // Checks to make sure the user didn't forget to add the 
            // floor prefab that will be used to tile the floor
            if (m_BottomEdgePrefab != null && m_HorizontalFillPrefab != null && m_VerticalFillPrefab != null && m_TopEdgePrefab != null)
            {
                m_CurrentlyConnected = new List<Wall>();

                m_OtherWalls = new List<Wall>();
                m_OtherWalls.AddRange(FindObjectsOfType<Wall>());
                m_OtherWalls.Remove(this);

                m_Touching = new Dictionary<Orientation, Wall>();

                foreach (Wall Other in m_OtherWalls)
                {
                    if (transform.position.y > Other.transform.position.y && transform.position.x == Other.transform.position.x)
                    {
                        if (m_VerticalPosition == Orientation.BOTTOM)
                        {
                            m_VerticalPosition = Orientation.CENTER;
                            Debug.Log(gameObject.name + " is a center");
                        }
                        else
                        {
                            m_VerticalPosition = Orientation.TOP;
                            m_Touching[Orientation.BOTTOM] = Other;
                        }
                    }
                    if (transform.position.y < Other.transform.position.y && transform.position.x == Other.transform.position.x)
                    {
                        if (m_VerticalPosition == Orientation.TOP)
                        {
                            m_VerticalPosition = Orientation.CENTER;
                            Debug.Log(gameObject.name + " is a center");
                        }
                        else
                        {
                            m_VerticalPosition = Orientation.BOTTOM;
                            m_Touching[Orientation.TOP] = Other;
                        }
                    }

                    if (transform.position.x > Other.transform.position.x && transform.position.y == Other.transform.position.y)
                    {
                        if (m_HorizontalPosition == Orientation.LEFT)
                            m_HorizontalPosition = Orientation.CENTER;
                        else
                        {
                            m_HorizontalPosition = Orientation.RIGHT;
                            m_Touching[Orientation.LEFT] = Other;
                        }
                    }
                    if (transform.position.x < Other.transform.position.x && transform.position.y == Other.transform.position.y)
                    {
                        if (m_HorizontalPosition == Orientation.RIGHT)
                            m_HorizontalPosition = Orientation.CENTER;
                        else
                            m_HorizontalPosition = Orientation.LEFT;
                    }
                }

                GameObject TempObject;

                if (m_VerticalPosition == Orientation.CENTER)
                {
                    TempObject = Instantiate(m_HorizontalFillPrefab,
                        new Vector3(
                            transform.position.x,
                            transform.position.y, 0.0f),
                        Quaternion.identity) as GameObject;
                    TempObject.transform.parent = transform;
                }
                if (m_VerticalPosition == Orientation.BOTTOM && m_VerticalPosition != Orientation.CENTER)
                {
                    TempObject = Instantiate(m_BottomEdgePrefab,
                        new Vector3(
                            transform.position.x,
                            transform.position.y, 0.0f),
                        Quaternion.identity) as GameObject;
                    TempObject.transform.parent = transform;
                }
                if (m_HorizontalPosition == Orientation.CENTER)
                {
                    TempObject = Instantiate(m_VerticalFillPrefab,
                        new Vector3(
                            transform.position.x,
                            transform.position.y, 0.0f),
                        Quaternion.identity) as GameObject;
                    TempObject.transform.parent = transform;
                }
                if (m_VerticalPosition == Orientation.TOP && m_VerticalPosition != Orientation.CENTER)
                {
                    TempObject = Instantiate(m_TopEdgePrefab,
                        new Vector3(
                            transform.position.x,
                            transform.position.y, 0.0f),
                        Quaternion.identity) as GameObject;
                    TempObject.transform.parent = transform;
                }

                if (EditorApplication.isPlaying)
                    m_SpriteRenderer.sprite = null;
            }
            else
            {
                Debug.LogError(name + " is missing a prefab to tile!\nProviding visual confirmation...");

                // Display a different sprite when there is no prefab so that 
                // it is easier to determine which object has an issue
                m_SpriteRenderer.sprite = Resources.Load<Sprite>("NULL_SPRITE");
                m_SpriteRenderer.sprite.texture.filterMode = FilterMode.Point;
            }
            if (EditorApplication.isPlaying)
                InstantiateWalls();
        }

        [ContextMenu("Instantiate Walls")]
        void InstantiateWalls()
        {
            foreach (Wall CurrentlyConnected in m_CurrentlyConnected)
                foreach (KeyValuePair<Orientation, Wall> Item in m_Touching.Where(x => x.Value == CurrentlyConnected).ToList())
                    m_Touching.Remove(Item.Key);

            foreach (KeyValuePair<Orientation, Wall> ConnectingWall in m_Touching)
            {
                if (ConnectingWall.Key == Orientation.TOP)
                {
                    for (int i = 1; i < Mathf.Abs(transform.position.y - ConnectingWall.Value.transform.position.y) * 100 / 16; ++i)
                    {
                        GameObject TempObject = Instantiate(m_VerticalFillPrefab,
                            new Vector3(
                                transform.position.x,
                                transform.position.y + i * 0.16f, 0.0f),
                            Quaternion.identity) as GameObject;
                        TempObject.transform.parent = transform;
                    }
                }
                if (ConnectingWall.Key == Orientation.LEFT)
                {
                    for (int i = 1; i < Mathf.Abs(transform.position.x - ConnectingWall.Value.transform.position.x) * 100 / 16; ++i)
                    {                      
                        GameObject TempObject = Instantiate(m_HorizontalFillPrefab,
                            new Vector3(
                                transform.position.x - i * 0.16f,
                                transform.position.y, 0.0f),
                            Quaternion.identity) as GameObject;
                        TempObject.transform.parent = transform;
                    }
                }
                //GameObject TempObject = Instantiate(m_PrefabToTile,
                //        new Vector3(
                //            transform.position.x,
                //            transform.position.y, 0.0f),
                //        Quaternion.identity) as GameObject;
                //TempObject.transform.parent = transform;

                //for (int i = 1; i < (Mathf.Abs(transform.position.x - Other.transform.position.x) * 100 / 16) - 1; ++i)
                //{
                //    TempObject = Instantiate(m_PrefabToTile,
                //        new Vector3(
                //            transform.position.x + i * 0.16f,
                //            transform.position.y, 0.0f),
                //        Quaternion.identity) as GameObject;
                //    TempObject.transform.parent = transform;
                //}
                ConnectingWall.Value.m_CurrentlyConnected.Add(this);
            }

            //m_SpriteRenderer.sprite = null; // Stop displaying this object's sprite since we don't need it anymore
        }
    }
}