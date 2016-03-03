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
        protected Vector3 m_LineOffset;
        [SerializeField]
        protected Vector3 m_LineLength;
        [SerializeField]
        protected Vector3 m_LineNum;

        static private Controller s_Self;
        static private List<GameObject>[,] s_Tiles;
        static private Vector2 s_IndexOffset;

        public bool ShowGrid { get { return m_ShowGrid; } }
        public Vector3 GridSize { get { return m_GridSize; } }
        public Vector4 GridColor { get { return m_GridColor; } }

        static public Controller Self { get { return s_Self; } set { s_Self = value; } }
        static public List<GameObject>[,] Tiles { get { return s_Tiles; } }

        protected override void OnEditorStart() { }
        protected override void OnGameStart()
        {
            gameObject.SetActive(false);
        }

        protected override void OnEditorUpdate()
        {
            while (FindObjectsOfType<Controller>().Length != 1)
            {
                Debug.LogWarning("You cannot have more than one Controller object at a time. Deleted.");
                DestroyImmediate(FindObjectOfType<Controller>().gameObject);
            }
            if (s_Self == null)
                s_Self = FindObjectOfType<Controller>();
        }
        protected override void OnEditorUpdateSelected() { }
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

            //Debug.Log(mousePosInScene);
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

        [MenuItem("TileMapper/Create Array")]
        static public void CreateArray()
        {
            List<GameObject> GameObjects = new List<GameObject>();

            GameObjects.AddRange(GameObject.FindGameObjectsWithTag("GameTile"));

            int HighestX = 0;
            int LowestX = 0;
            int HighestY = 0;
            int LowestY = 0;
            foreach (GameObject gameObject in GameObjects)
            {
                if (gameObject.transform.position.x / s_Self.m_UnitGridSize.x > HighestX)
                    HighestX = (int)(gameObject.transform.position.x / s_Self.m_UnitGridSize.x);
                if (gameObject.transform.position.x / s_Self.m_UnitGridSize.x < LowestX)
                    LowestX = (int)(gameObject.transform.position.x / s_Self.m_UnitGridSize.x);
                if (gameObject.transform.position.y / s_Self.m_UnitGridSize.y > HighestY)
                    HighestY = (int)(gameObject.transform.position.y / s_Self.m_UnitGridSize.y);
                if (gameObject.transform.position.y / s_Self.m_UnitGridSize.y < LowestY)
                    LowestY = (int)(gameObject.transform.position.y / s_Self.m_UnitGridSize.y);
            }
            s_Tiles = new List<GameObject>[HighestX + Math.Abs(LowestX) + 1, HighestY + Math.Abs(LowestY) + 1];

            for (int i = 0; i < s_Tiles.GetLength(0); ++i)
                for (int j = 0; j < s_Tiles.GetLength(1); ++j)
                    s_Tiles[i, j] = new List<GameObject>();

            s_IndexOffset = new Vector2(Mathf.Abs(LowestX), Mathf.Abs(LowestY));
            for (int i = 0; i < GameObjects.Count; ++i)
            {
                int j = (int)Mathf.Round(GameObjects[i].transform.position.x / s_Self.m_UnitGridSize.x) + (int)s_IndexOffset.x;
                int k = (int)Mathf.Round(GameObjects[i].transform.position.y / s_Self.m_UnitGridSize.y) + (int)s_IndexOffset.y;

                s_Tiles[j, k].Add(GameObjects[i]);
            }
        }
        static public Vector2 ConvertToIndex(Vector3 a_Pos)
        {
            return new Vector2(
                Mathf.Round(a_Pos.x / s_Self.m_UnitGridSize.x) + s_IndexOffset.x,
                Mathf.Round(a_Pos.y / s_Self.m_UnitGridSize.y) + s_IndexOffset.y);
        }

        static public void AddTile(ref Tile a_Tile)
        {
            s_Tiles[(int)a_Tile.TileIndex.x, (int)a_Tile.TileIndex.y].Add(a_Tile.gameObject);
        }
        static public void AddTile(ref GameObject a_Object, Vector2 a_Index)
        {
            s_Tiles[(int)a_Index.x, (int)a_Index.y].Add(a_Object.gameObject);
        }
        static public void RemoveTile(Tile a_Tile)
        {
            s_Tiles[(int)a_Tile.TileIndex.x, (int)a_Tile.TileIndex.y].Remove(a_Tile.gameObject);
        }
        static public void RemoveTile(GameObject a_Object, Vector2 a_Index)
        {
            s_Tiles[(int)a_Index.x, (int)a_Index.y].Remove(a_Object.gameObject);
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
            if ((a_Object.transform.position.x + a_Offset.x) % a_GridSize.x != 0 ||
                (a_Object.transform.position.y + a_Offset.y) % a_GridSize.y != 0 ||
                (a_Object.transform.position.z + a_Offset.z) % a_GridSize.z != 0)
            {
                // If the m_Gridsize isn't 0.0f for an axis, then snap it
                a_Object.transform.position = new Vector3(
                    (a_GridSize.x != 0.0f) ?
                        (int)((a_Object.transform.position.x - a_Offset.x + ((a_Object.transform.position.x > 0) ? a_GridSize.x / 2.0f : a_GridSize.x / -2.0f)) / a_GridSize.x) * a_GridSize.x + a_Offset.x : a_Object.transform.position.x + a_Offset.x,
                    (a_GridSize.y != 0.0f) ?
                        (int)((a_Object.transform.position.y - a_Offset.x + ((a_Object.transform.position.y > 0) ? a_GridSize.y / 2.0f : a_GridSize.y / -2.0f)) / a_GridSize.y) * a_GridSize.y + a_Offset.y : a_Object.transform.position.y + a_Offset.y,
                    (a_GridSize.z != 0.0f) ?
                        (int)((a_Object.transform.position.z - a_Offset.x + ((a_Object.transform.position.z > 0) ? a_GridSize.z / 2.0f : a_GridSize.z / -2.0f)) / a_GridSize.z) * a_GridSize.z + a_Offset.z : a_Object.transform.position.z + a_Offset.z);

                // Avoid floating point error. Unnecessary in some cases, but for the most part having 0.080000001 is unacceptable
                // Converts something like that to 0.08, which is correct.                        
                a_Object.transform.localPosition = new Vector3(
                            Mathf.Floor(a_Object.transform.localPosition.x * 100.0f) / 100.0f,
                            Mathf.Floor(a_Object.transform.localPosition.y * 100.0f) / 100.0f,
                            Mathf.Floor(a_Object.transform.localPosition.z * 100.0f) / 100.0f);
            }
        }
    }
}
