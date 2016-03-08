using UnityEngine;
using UnityEditor;
using UnityEditorInternal;
using System;
using System.Reflection;
using System.Linq;
using System.Collections.Generic;
using Bennybroseph.System;

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
        [SerializeField, Tooltip("The color of the grid\nAcceptable range is 0-1 using floating point numbers")]
        protected Vector4 m_GridColor;

        [SerializeField]
        protected Vector3 m_LineOffset;
        [SerializeField]
        protected Vector3 m_LineLength;
        [SerializeField]
        protected Vector3 m_LineNum;

        static private Controller s_Self;
        static private Dictionary<Vector2<int>, List<GameObject>> s_Tiles;
        static private Vector2<int> s_TilemapOffset;
        static private Vector2<int> s_TilemapSize;

        static private List<string> s_SortingLayers;

        public bool ShowGrid { get { return m_ShowGrid; } }
        public Vector3 PixelGridSize { get { return m_GridSize; } }
        public Vector3 GridSize { get { return m_UnitGridSize; } }
        public Vector4 GridColor { get { return m_GridColor; } }

        static public Controller Self { get { return s_Self; } set { s_Self = value; } }
        static public Dictionary<Vector2<int>, List<GameObject>> Tiles { get { return s_Tiles; } }
        static public Vector2<int> TilemapSize { get { return s_TilemapSize; } }
        static public List<string> SortingLayers { get { return s_SortingLayers; } }

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
            m_GridSize = new Vector3(
                Mathf.Clamp(m_GridSize.x, 0, m_GridSize.x),
                Mathf.Clamp(m_GridSize.y, 0, m_GridSize.y),
                Mathf.Clamp(m_GridSize.z, 0, m_GridSize.z));

            m_UnitGridSize = new Vector3(m_GridSize.x / 100.0f, m_GridSize.y / 100.0f, m_GridSize.z / 100.0f);
        }

        static public void CacheSortingLayers()
        {
            s_SortingLayers = new List<string>();
            // Grabs all sorting layers from Unity
            // They are in the same order in the list s_SortingLayers as the user places them in the editor
            Type internalEditorUtilityType = typeof(InternalEditorUtility);
            PropertyInfo sortingLayersProperty = internalEditorUtilityType.GetProperty("sortingLayerNames", BindingFlags.Static | BindingFlags.NonPublic);
            s_SortingLayers.AddRange((string[])sortingLayersProperty.GetValue(null, new object[0]));

            s_SortingLayers.Sort();
        }

        [MenuItem("TileMapper/Create Array")]
        static public void CreateArray()
        {
            CacheSortingLayers();

            List<GameObject> GameObjects = new List<GameObject>();
            s_TilemapSize = new Vector2<int>(0, 0);

            GameObjects.AddRange(GameObject.FindGameObjectsWithTag("GameTile").OrderBy(gameObject => GetSortingLayerOrder(gameObject.GetComponent<SpriteRenderer>().sortingLayerName)).ToList());

            int LowestX = 0, HighestX = 0;
            int LowestY = 0, HighestY = 0;
            foreach (GameObject gameObject in GameObjects)
            {
                Vector3<int> ObjectGridPosition = new Vector3<int>((int)GetGridPosition(gameObject.transform.position).x, (int)GetGridPosition(gameObject.transform.position).y, 0);

                if (ObjectGridPosition.x < LowestX)
                    LowestX = ObjectGridPosition.x;
                if (ObjectGridPosition.x > HighestX)
                    HighestX = ObjectGridPosition.x;

                if (ObjectGridPosition.y < LowestY)
                    LowestY = ObjectGridPosition.y;
                if (ObjectGridPosition.y > HighestY)
                    HighestY = ObjectGridPosition.y;
            }
            s_TilemapOffset = new Vector2<int>(LowestX, LowestY);
            s_TilemapSize = new Vector2<int>(Math.Abs(HighestX - LowestX) + 1, Math.Abs(HighestY - LowestY) + 1);

            s_Tiles = new Dictionary<Vector2<int>, List<GameObject>>();
            foreach (GameObject gameObject in GameObjects)
            {
                Vector2<int> ArrayIndex = GetArrayIndex(gameObject);
                if (!s_Tiles.ContainsKey(ArrayIndex))
                    s_Tiles[ArrayIndex] = new List<GameObject>();
                
                s_Tiles[ArrayIndex].Add(gameObject);
            }
            //s_Tiles = new List<GameObject>[Math.Abs(HighestX) + Math.Abs(LowestX) + 1, Math.Abs(HighestY) + Math.Abs(LowestY) + 1];
        }

        static public Vector2<int> GetArrayIndex(GameObject a_Object)
        {
            if (a_Object.GetComponent<Tile>() != null)
                return GetArrayIndex(a_Object.transform.position, a_Object.GetComponent<Tile>().GridSize, a_Object.GetComponent<Tile>().Offset);
            else
                return GetArrayIndex(a_Object.transform.position, s_Self.m_UnitGridSize, Vector3.zero);

        }
        static public Vector2<int> GetArrayIndex(Vector3 a_Pos, Vector3 a_GridSize, Vector3 a_Offset)
        {
            Vector2 GridPosition = GetGridPosition(a_Pos, a_GridSize, a_Offset);

            Vector2<int> ReturnValue = new Vector2<int>(
                (int)GridPosition.x - s_TilemapOffset.x,
                (int)GridPosition.y - s_TilemapOffset.y);

            ReturnValue = new Vector2<int>(ReturnValue.x, (s_TilemapSize.y - 1)  - ReturnValue.y);
            return ReturnValue;
        }

        // Allows you to only pass a position and let the grid size be based on the Controller object's values
        static public Vector3 GetGridPosition(Vector3 a_Position)
        {
            return GetGridPosition(a_Position, s_Self.m_UnitGridSize, Vector3.zero);
        }
        // Takes a position, and normalizes it over a grid size and offset
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

        static public int GetSortingLayerOrder(string a_SortingLayer)
        {
            for (int i = 0; i < s_SortingLayers.Count; ++i)
            {
                if (s_SortingLayers[i] == a_SortingLayer)
                    return i;
            }

            Debug.LogError("GetSortingLayerOrder('" + a_SortingLayer + "') will return -1.\n'" + a_SortingLayer + "' does not exist in the list");
            return -1;
        }

        static public void AddTile(Tile a_Tile)
        {
            s_Tiles[GetArrayIndex(a_Tile.gameObject)].Add(a_Tile.gameObject);
        }
        static public void AddTile(GameObject a_Object, Vector2 a_Index)
        {
            s_Tiles[GetArrayIndex(a_Object)].Add(a_Object);
        }
        static public void RemoveTile(Tile a_Tile)
        {
            s_Tiles[GetArrayIndex(a_Tile.gameObject)].Remove(a_Tile.gameObject);
        }
        static public void RemoveTile(GameObject a_Object, Vector2 a_Index)
        {
            s_Tiles[GetArrayIndex(a_Object)].Remove(a_Object);
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
