using UnityEngine;
using System.Collections;

namespace TileMapper
{
    // Everything will most-likely have this as a base class since everything is going to be tile based
    public class Tile : MonoBehaviour
    {
        [SerializeField, Tooltip("Whether the tile should snap to the grid or not")]
        protected bool m_ShouldSnap;
        
        [SerializeField, Tooltip("What grid size the tile should snap to.\nA '0' in any field indicates no snapping for that axis\n\nValues are in PIXELS not units, because units are confusing in pixel based games\n1 unit = 100 pixels")]
        protected Vector3 m_GridSize;
        [SerializeField, Tooltip("After snapping to the grid, the values in this variable will be added on to its current position\n\nValues are in PIXELS not units, because units are confusing in pixel based games\n1 unit = 100 pixels")]
        protected Vector3 m_Offset;
        
        protected virtual void Start()
        {
            
        }

        // Called when something changes in the inspector
        public virtual void SnapToGrid()
        {
            if (m_ShouldSnap)
            {
                // Determine whether the object is currently snapped
                if (transform.position.x * 100 % (m_GridSize.x + m_Offset.x) != 0 ||
                    transform.position.y * 100 % (m_GridSize.y + m_Offset.y) != 0 ||
                    transform.position.z * 100 % (m_GridSize.z + m_Offset.z) != 0)
                {
                    // If the m_Gridsize isn't 0.0f for an axis, then snap it
                    transform.position = new Vector3(
                        (m_GridSize.x != 0.0f) ? Mathf.Round(transform.position.x * 100.0f / m_GridSize.x) * m_GridSize.x / 100.0f + m_Offset.x / 100.0f : transform.position.x + m_Offset.x / 100.0f,
                        (m_GridSize.y != 0.0f) ? Mathf.Round(transform.position.y * 100.0f / m_GridSize.y) * m_GridSize.y / 100.0f + m_Offset.y / 100.0f : transform.position.y + m_Offset.y / 100.0f,
                        (m_GridSize.z != 0.0f) ? Mathf.Round(transform.position.z * 100.0f / m_GridSize.z) * m_GridSize.z / 100.0f + m_Offset.z / 100.0f : transform.position.z + m_Offset.z / 100.0f);
                }
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
        }
    }
}
