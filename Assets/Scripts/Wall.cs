using UnityEngine;
using System.Collections.Generic;

namespace TileMapper
{
    class Wall : MonoBehaviour
    {
        public enum Type { CENTER, CENTER_EDGE, FILL, FILL_EDGE};

        [SerializeField]
        protected GameObject m_CenterPrefab;
        [SerializeField]
        protected GameObject m_CenterEdgePrefab;
        [SerializeField]
        protected GameObject m_FillPrefab;
        [SerializeField]
        protected GameObject m_FillEdgePrefab;

        [SerializeField]
        protected Type m_Type;

        List<Wall> m_OtherWalls = new List<Wall>();

        // The wall objects which have already connected to this one
        protected List<Wall> m_CurrentlyConnected = new List<Wall>();
        protected SpriteRenderer m_SpriteRenderer;

        // Used to instantiate the requred objects to fill the space

        [ContextMenu("Connect Walls")]
        public void ConnectWalls()
        {
            // Checks to make sure the user didn't forget to add a sprite renderer
            // One is not required, but recommended for error checking
            if ((m_SpriteRenderer = GetComponent<SpriteRenderer>()) == null)
                Debug.LogWarning(name + " has no sprite renderer!");

            // Checks to make sure the user didn't forget to add the 
            // floor prefab that will be used to tile the floor
            if (m_CenterEdgePrefab != null && m_CenterPrefab != null && m_FillPrefab != null && m_FillEdgePrefab != null)
            {
                
                m_OtherWalls.AddRange(FindObjectsOfType<Wall>());

                m_OtherWalls.Remove(this);

                m_Type = Type.CENTER_EDGE;
                foreach (Wall Other in m_OtherWalls)
                {
                    if (transform.position.y < Other.transform.position.y && transform.position.x == Other.transform.position.x)
                        m_Type = Type.CENTER_EDGE;
                    if (transform.position.y > Other.transform.position.y && transform.position.x == Other.transform.position.x)
                        m_Type = Type.FILL_EDGE;

                    //if (transform.position.x < Other.transform.position.x && transform.position.y == Other.transform.position.y)
                    //    m_Type = Type.CENTER_EDGE;
                }

                GameObject TempObject = new GameObject();

                if (m_Type == Type.CENTER)
                {
                    TempObject = Instantiate(m_CenterPrefab,
                        new Vector3(
                            transform.position.x,
                            transform.position.y, 0.0f),
                        Quaternion.identity) as GameObject;
                }
                if (m_Type == Type.CENTER_EDGE)
                {
                    TempObject = Instantiate(m_CenterEdgePrefab,
                        new Vector3(
                            transform.position.x,
                            transform.position.y, 0.0f),
                        Quaternion.identity) as GameObject;
                }
                if (m_Type == Type.FILL)
                {
                    TempObject = Instantiate(m_FillPrefab,
                        new Vector3(
                            transform.position.x,
                            transform.position.y, 0.0f),
                        Quaternion.identity) as GameObject;
                }
                if (m_Type == Type.FILL_EDGE)
                {
                    TempObject = Instantiate(m_FillEdgePrefab,
                        new Vector3(
                            transform.position.x,
                            transform.position.y, 0.0f),
                        Quaternion.identity) as GameObject;
                }
                TempObject.transform.parent = transform;
            }
            else
            {
                Debug.LogError(name + " is missing a prefab to tile!\nProviding visual confirmation...");

                // Display a different sprite when there is no prefab so that 
                // it is easier to determine which object has an issue
                m_SpriteRenderer.sprite = Resources.Load<Sprite>("NULL_SPRITE");
                m_SpriteRenderer.sprite.texture.filterMode = FilterMode.Point;
            }
        }

        [ContextMenu("Instantiate Walls")]
        void InstantiateWalls()
        {
            foreach (Wall CurrentlyConnected in m_CurrentlyConnected)
                m_OtherWalls.Remove(CurrentlyConnected);

            //foreach (Wall Other in OtherWalls)
            //{
            //    GameObject TempObject = Instantiate(m_PrefabToTile,
            //            new Vector3(
            //                transform.position.x,
            //                transform.position.y, 0.0f),
            //            Quaternion.identity) as GameObject;
            //    TempObject.transform.parent = transform;

            //    for (int i = 1; i < (Mathf.Abs(transform.position.x - Other.transform.position.x) * 100 / 16) - 1; ++i)
            //    {
            //        TempObject = Instantiate(m_PrefabToTile,
            //            new Vector3(
            //                transform.position.x + i * 0.16f,
            //                transform.position.y, 0.0f),
            //            Quaternion.identity) as GameObject;
            //        TempObject.transform.parent = transform;
            //    }
            //    Other.m_CurrentlyConnected.Add(this);
            //}

            //m_SpriteRenderer.sprite = null; // Stop displaying this object's sprite since we don't need it anymore
        }
    }
}