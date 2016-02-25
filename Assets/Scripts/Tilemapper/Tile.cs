using UnityEngine;
using System.Collections;

namespace TileMapper
{
    public class Tile : MonoBehaviour
    {
        [SerializeField, Tooltip("Whether the tile should snap to the grid or not")]
        protected bool m_ShouldSnap;
        
        [SerializeField, Tooltip("What grid size the tile should snap to.\nA '0' in any field indicates no snapping for that axis")]
        protected Vector3 m_GridSize;
        [SerializeField, Tooltip("After snapping to the grid, the values in this variable will be added on to its current position")]
        protected Vector3 m_Offset;
        
        // Called when something changes in the inspector
        protected virtual void SnapToGrid()
        {
            // Snap to the grid
            if (transform.position.x * 100 % (m_GridSize.x + m_Offset.x) != 0 ||
                transform.position.y * 100 % (m_GridSize.y + m_Offset.y) != 0 ||
                transform.position.z * 100 % (m_GridSize.z + m_Offset.z) != 0)
            {
                // If the grid size isn't 0.0f for the axis, then snap it
                transform.position = new Vector3(
                    (m_GridSize.x != 0.0f) ? Mathf.Round(transform.position.x * 100.0f / m_GridSize.x) * m_GridSize.x / 100.0f + m_Offset.x : transform.position.x + m_Offset.x,
                    (m_GridSize.y != 0.0f) ? Mathf.Round(transform.position.y * 100.0f / m_GridSize.y) * m_GridSize.y / 100.0f + m_Offset.y : transform.position.y + m_Offset.y,
                    (m_GridSize.z != 0.0f) ? Mathf.Round(transform.position.z * 100.0f / m_GridSize.z) * m_GridSize.z / 100.0f + m_Offset.z : transform.position.z + m_Offset.z);
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
