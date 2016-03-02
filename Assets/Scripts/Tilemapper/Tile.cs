using UnityEngine;
using UnityEditor;
using System.Collections;
using System;

namespace TileMapper
{
    // Everything will most-likely have this as a base class since everything is going to be tile based
    public class Tile : ExecuteInEditor
    {
        [SerializeField, Tooltip("Whether the tile should snap to the grid or not")]
        protected bool m_ShouldSnap;

        [SerializeField, Tooltip("What grid size the tile should snap to\nA '0' in any field indicates no snapping for that axis\n\nValues are in PIXELS not units, because units are confusing in pixel based games\n1 unit = 100 pixels")]
        protected Vector3 m_GridSize;
        // The grid size in Unity units
        protected Vector3 m_UnitGridSize;
        [SerializeField, ReadOnly, Tooltip("The index of the tile in the static s_Tiles array of TileMapper.Controller\nCannot be edited, but is shown")]
        protected Vector2 m_TileIndex;
        [SerializeField, Tooltip("After snapping to the grid, the values in this variable will be added on to its current position\n\nValues are in PIXELS not units, because units are confusing in pixel based games\n1 unit = 100 pixels")]
        protected Vector3 m_Offset;
        // The offset in Unity units
        protected Vector3 m_UnitOffset;

        public bool ShouldSnap { get { return m_ShouldSnap; } }
        public Vector3 GridSize { get { return m_UnitGridSize; } }
        public Vector2 TileIndex { get { return m_TileIndex; } }
        public Vector3 Offset { get { return m_UnitOffset; } }

        protected virtual void OnValidate()
        {
            if (m_GridSize.x < 0.0f)
                m_GridSize = new Vector3(0.0f, m_GridSize.y, m_GridSize.z);
            if (m_GridSize.y < 0.0f)
                m_GridSize = new Vector3(m_GridSize.x, 0.0f, m_GridSize.z);
            if (m_GridSize.z < 0.0f)
                m_GridSize = new Vector3(m_GridSize.x, m_GridSize.y, 0.0f);

            // If the grid size to snap to is 0, get it from the controller
            // If you don't want the object to snap, turn off snapping
            if (m_GridSize == Vector3.zero)
                m_GridSize = Controller.Self.GridSize;

            m_UnitGridSize = new Vector3(m_GridSize.x / 100.0f, m_GridSize.y / 100.0f, m_GridSize.z / 100.0f);
            m_UnitOffset = new Vector3(m_Offset.x / 100.0f, m_Offset.y / 100.0f, m_Offset.z / 100.0f);
        }

        // Almost purely virtual functions. I would have this as an abstract class
        // but then everything that inherits would need to implement these functions
        // and I don't think that's necessary. Plus an object may need to be
        // created of type Tile, and this is not possible on an abstract class
        protected override void OnEditorStart() { }
        protected override void OnEditorUpdate() { }
        protected override void OnEditorUpdateSelected() { Controller.SnapToGrid(this); }
        protected override void OnGameStart() { }
        protected override void OnGameUpdate() { }
    }
}
