using UnityEngine;
using UnityEditor;
using System.Linq;
using System.Collections.Generic;
using System;

namespace TileMapper
{
    public class Controller : ExecuteInEditor
    {
        [SerializeField]
        protected bool m_ShowGrid;
        [SerializeField, Tooltip("What grid size to draw\n\nValues are in PIXELS not units, because units are confusing in pixel based games\n1 unit = 100 pixels")]
        protected Vector3 m_GridSize;
        // The grid size in Unity units
        protected Vector3 m_UnitGridSize;
        [SerializeField]
        protected Vector4 m_GridColor;

        [SerializeField]
        public Vector3 m_LineOffset;
        [SerializeField]
        public Vector3 m_LineLength;
        [SerializeField]
        public Vector3 m_LineNum;

        [SerializeField]
        protected List<Tile> m_Tiles;

        static private Controller s_Self;

        public bool ShowGrid { get { return m_ShowGrid; } }
        public Vector3 GridSize { get { return m_UnitGridSize; } }
        public Vector4 GridColor { get { return m_GridColor; } }

        static public Controller Self { get { return s_Self; } }

        protected override void OnEditorStart()
        {
            while (FindObjectsOfType<Controller>().Length != 1)
                DestroyImmediate(FindObjectOfType<Controller>());
            s_Self = FindObjectOfType<Controller>();
        }
        protected override void OnGameStart()
        {
            gameObject.SetActive(false);
        }

        protected override void OnEditorUpdate()
        {
            m_Tiles = new List<Tile>();
            m_Tiles.AddRange(FindObjectsOfType<Tile>());

            foreach (Tile tile in m_Tiles)
                tile.SnapToGrid();
        }
        protected override void OnGameUpdate() { }

        protected virtual void OnDrawGizmos()
        {
            if (m_ShowGrid)
            {
                Vector3 CameraPos = Camera.current.transform.position;
                
                Vector3 ScaledGridSize = m_UnitGridSize;
                if (CameraPos.z < -20)
                    ScaledGridSize *= 2 * Mathf.Floor(CameraPos.z / -20);

                Vector3 Start = new Vector3(
                    Mathf.Round((CameraPos.x - (Mathf.Abs(CameraPos.z) * m_LineOffset.x)) / ScaledGridSize.x) * ScaledGridSize.x + m_UnitGridSize.x / 2.0f,
                    Mathf.Round((CameraPos.y - (Mathf.Abs(CameraPos.z) * m_LineOffset.y)) / ScaledGridSize.y) * ScaledGridSize.y + m_UnitGridSize.y / 2.0f);
                Vector3 End = new Vector3(Start.x + (Mathf.Abs(CameraPos.z) * m_LineLength.x), Start.y + (Mathf.Abs(CameraPos.z) * m_LineLength.y));

                Gizmos.color = m_GridColor;
                for (int i = 0; i < 5 + Mathf.Abs(CameraPos.z) * m_LineNum.x; ++i)
                    Gizmos.DrawLine(new Vector3(Start.x + i * ScaledGridSize.x, Start.y), new Vector3(Start.x + i * ScaledGridSize.x, End.y));

                for (int i = 0; i < 5 + Mathf.Abs(CameraPos.z) * m_LineNum.y; ++i)
                    Gizmos.DrawLine(new Vector3(Start.x, Start.y + i * ScaledGridSize.y), new Vector3(End.x, Start.y + i * ScaledGridSize.y));
            }
        }

        protected virtual void OnValidate()
        {
            if (m_GridSize.x < 0.0f)
                m_GridSize = new Vector3(0.0f, m_GridSize.y, m_GridSize.z);
            if (m_GridSize.y < 0.0f)
                m_GridSize = new Vector3(m_GridSize.x, 0.0f, m_GridSize.z);
            if (m_GridSize.z < 0.0f)
                m_GridSize = new Vector3(m_GridSize.x, m_GridSize.y, 0.0f);

            m_UnitGridSize = new Vector3(m_GridSize.x / 100.0f, m_GridSize.y / 100.0f, m_GridSize.z / 100.0f);
        }

        // Easy way to snap a tile object
        static public void SnapToGrid(Tile a_Tile)
        {
            if (a_Tile.ShouldSnap)
                SnapToGrid(a_Tile.gameObject, a_Tile.GridSize, a_Tile.Offset);
        }
        // Allows snapping based on the Editor grid. Useful, but uses some bad singleton code
        static public void SnapToGrid(GameObject a_Object)
        {
            SnapToGrid(a_Object, s_Self.m_GridSize, new Vector3(0, 0, 0));
        }
        // Useful to keep a snap function outside of the Tile object so that anything can snap to the grid, not just tiles should it be desired
        static public void SnapToGrid(GameObject a_Object, Vector3 a_GridSize, Vector3 a_Offset)
        {
            // Determine whether the object is currently snapped
            if (a_Object.transform.position.x % (a_GridSize.x + a_Offset.x) != 0 ||
                a_Object.transform.position.y % (a_GridSize.y + a_Offset.y) != 0 ||
                a_Object.transform.position.z % (a_GridSize.z + a_Offset.z) != 0)
            {
                // If the m_Gridsize isn't 0.0f for an axis, then snap it
                a_Object.transform.position = new Vector3(
                    (a_GridSize.x != 0.0f) ? Mathf.Round(a_Object.transform.position.x / a_GridSize.x) * a_GridSize.x + a_Offset.x : a_Object.transform.position.x + a_Offset.x,
                    (a_GridSize.y != 0.0f) ? Mathf.Round(a_Object.transform.position.y / a_GridSize.y) * a_GridSize.y + a_Offset.y : a_Object.transform.position.y + a_Offset.y,
                    (a_GridSize.z != 0.0f) ? Mathf.Round(a_Object.transform.position.z / a_GridSize.z) * a_GridSize.z + a_Offset.z : a_Object.transform.position.z + a_Offset.z);
            }
        }
    }
}
