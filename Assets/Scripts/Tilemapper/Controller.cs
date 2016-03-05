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
        public Vector3 GridSize { get { return m_UnitGridSize; } }
        public Vector4 GridColor { get { return m_GridColor; } }

        static public Controller Self { get { return s_Self; } set { s_Self = value; } }
        static public List<GameObject>[,] Tiles { get { return s_Tiles; } }

        protected override void OnEditorStart()
        {
            while (FindObjectsOfType<Controller>().Length != 1)
            {
                Debug.LogWarning("You cannot have more than one Controller object at a time. Deleted.");
                DestroyImmediate(FindObjectOfType<Controller>().gameObject);
            }
            if (s_Self == null)
                s_Self = FindObjectOfType<Controller>();

            CreateArray();
        }
        protected override void OnGameStart()
        {
            gameObject.SetActive(false);
        }

        protected override void OnEditorUpdate() { }
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
            s_IndexOffset = Vector2.zero;

            GameObjects.AddRange(GameObject.FindGameObjectsWithTag("GameTile"));

            int HighestX = 0, LowestX = 0;
            int HighestY = 0, LowestY = 0;
            foreach (GameObject gameObject in GameObjects)
            {
                Vector3 ObjectGridPosition = GetGridPosition(gameObject.transform.position);

                if ((int)ObjectGridPosition.x > HighestX)
                    HighestX = (int)ObjectGridPosition.x;
                if ((int)ObjectGridPosition.x < LowestX)
                    LowestX  = (int)ObjectGridPosition.x;
                if ((int)ObjectGridPosition.y > HighestY)
                    HighestY = (int)ObjectGridPosition.y;
                if ((int)ObjectGridPosition.y < LowestY)
                    LowestY  = (int)ObjectGridPosition.y;
            }
            s_Tiles = new List<GameObject>[Math.Abs(HighestX) + Math.Abs(LowestX) + 1, Math.Abs(HighestY) + Math.Abs(LowestY) + 1];

            for (int i = 0; i < s_Tiles.GetLength(0); ++i)
                for (int j = 0; j < s_Tiles.GetLength(1); ++j)
                    s_Tiles[i, j] = new List<GameObject>();

            
            s_IndexOffset = new Vector2(LowestX, LowestY);

            for (int i = 0; i < GameObjects.Count; ++i)
            {
                Vector2 ObjectArrayIndex = GetArrayIndex(GameObjects[i]);                

                s_Tiles[(int)ObjectArrayIndex.x, (int)ObjectArrayIndex.y].Add(GameObjects[i]);
            }
        }

        static public Vector2 GetArrayIndex(GameObject a_Object)
        {
            if (a_Object.GetComponent<Tile>() != null)
                return GetArrayIndex(a_Object.transform.position, a_Object.GetComponent<Tile>().GridSize, a_Object.GetComponent<Tile>().Offset);
            else
                return GetArrayIndex(a_Object.transform.position, s_Self.m_UnitGridSize, Vector3.zero);

        }
        static public Vector2 GetArrayIndex(Vector3 a_Pos, Vector3 a_GridSize, Vector3 a_Offset)
        {
            Vector2 GridPosition = GetGridPosition(a_Pos, a_GridSize, a_Offset); 

            return new Vector2(
                GridPosition.x - s_IndexOffset.x,
                GridPosition.y - s_IndexOffset.y);

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

        static public Vector3 GetGridPosition(Vector3 a_Position)
        {
            return GetGridPosition(a_Position, s_Self.m_UnitGridSize, Vector3.zero);
        }
        static public Vector3 GetGridPosition(Vector3 a_Position, Vector3 a_GridSize, Vector3 a_Offset)
        {
            // If the a_Gridsize isn't 0.0f for an axis, then snap it
            // The first ternary operator (? :) is to prevent division by 0
            // The second one makes its position in the grid based on its center 
            //ex. a position of (0.17, 0, 0) is (1, 0, 0) in grid space not (2, 0, 0) 
            return new Vector3(
                    (a_GridSize.x != 0.0f) ?
                        (int)((a_Position.x - a_Offset.x + ((a_Position.x > 0) ? a_GridSize.x / 2.0f : a_GridSize.x / -2.0f)) / a_GridSize.x) :
                        a_Position.x,
                    (a_GridSize.y != 0.0f) ?
                        (int)((a_Position.y - a_Offset.x + ((a_Position.y > 0) ? a_GridSize.y / 2.0f : a_GridSize.y / -2.0f)) / a_GridSize.y) :
                        a_Position.y,
                    (a_GridSize.z != 0.0f) ?
                        (int)((a_Position.z - a_Offset.x + ((a_Position.z > 0) ? a_GridSize.z / 2.0f : a_GridSize.z / -2.0f)) / a_GridSize.z) :
                        a_Position.z);
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
            SnapToGrid(a_Object, s_Self.m_UnitGridSize, Vector3.zero);
        }
        // Useful to keep a snap function outside of the Tile object so that any GameObject can snap to the grid, not just Tile objects should it be desired
        static public void SnapToGrid(GameObject a_Object, Vector3 a_GridSize, Vector3 a_Offset)
        {
            // Determine whether the object is currently snapped
            if ((a_Object.transform.position.x + a_Offset.x) % a_GridSize.x != 0 ||
                (a_Object.transform.position.y + a_Offset.y) % a_GridSize.y != 0 ||
                (a_Object.transform.position.z + a_Offset.z) % a_GridSize.z != 0)
            {
                // Get the object's position in the grid space
                a_Object.transform.position = GetGridPosition(a_Object.transform.position, a_GridSize, a_Offset);

                // This takes it's position on the grid and translates it into a world position
                // ex. (2, 1, 0) = (0.32f, 0.16f, 0) if the grid size is 0.16f
                // The ternary operator is to prevent any change when the grid size for a given axis is 0.0f
                a_Object.transform.position = new Vector3(
                    (a_GridSize.x != 0.0f) ?
                        a_Object.transform.position.x * a_GridSize.x + a_Offset.x :
                        a_Object.transform.position.x + a_Offset.x,
                    (a_GridSize.y != 0.0f) ?
                        a_Object.transform.position.y * a_GridSize.y + a_Offset.y :
                        a_Object.transform.position.y + a_Offset.y,
                    (a_GridSize.z != 0.0f) ?
                        a_Object.transform.position.z * a_GridSize.z + a_Offset.z :
                        a_Object.transform.position.z + a_Offset.z);

                //Avoid floating point error. Unnecessary in some cases, but for the most part having 0.080000001 is unacceptable
                //Converts something like that to 0.08, which is correct.
                //a_Object.transform.position = new Vector3(
                //    (int)(a_Object.transform.position.x * 100.0f) / 100.0f,
                //    (int)(a_Object.transform.position.y * 100.0f) / 100.0f,
                //    (int)(a_Object.transform.position.z * 100.0f) / 100.0f);
            }
        }
    }
}
