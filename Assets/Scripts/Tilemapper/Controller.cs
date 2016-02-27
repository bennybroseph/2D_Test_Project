using UnityEngine;
using UnityEditor;
using System.Linq;
using System.Collections.Generic;

namespace TileMapper
{
    [ExecuteInEditMode]
    public class Controller : MonoBehaviour
    {
        [SerializeField]
        protected bool m_ShowGrid;
        [SerializeField]
        protected Vector3 m_GridSize;
        [SerializeField]
        protected Vector4 m_GridColor;

        [SerializeField]
        protected List<Tile> m_Tiles;

        public bool ShowGrid { get { return m_ShowGrid; } }
        public Vector3 GridSize { get { return m_GridSize; } }
        public Vector4 GridColor { get { return m_GridColor; } }

        protected virtual void Start()
        {
            if (!EditorApplication.isPlaying)
                OnEditorStart();
            else
                OnGameStart();
        }

        protected virtual void Update()
        {
            if (!EditorApplication.isPlaying)
                OnEditorUpdate();
            else
                OnGameUpdate();
        }

        protected virtual void OnEditorStart()
        {

        }
        protected virtual void OnGameStart()
        {
            gameObject.SetActive(false);
        }

        protected virtual void OnEditorUpdate()
        {
            m_Tiles = new List<Tile>();
            m_Tiles.AddRange(FindObjectsOfType<Tile>());

            foreach (Tile tile in m_Tiles)
                tile.SnapToGrid();
        }
        protected virtual void OnGameUpdate()
        {

        }
        void OnDrawGizmos()
        {

        }
    }
}
