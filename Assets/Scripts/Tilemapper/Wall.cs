using UnityEngine;
using System.Collections;
using TileMapper;
using Bennybroseph.MySystem;

public class Wall : Tile
{
    [SerializeField]
    protected Sprite m_HorizontalCenter;
    [SerializeField]
    protected Sprite m_HorizontalEdge;
    [SerializeField]
    protected Sprite m_VerticalCenter;
    [SerializeField]
    protected Sprite m_VerticalEdge;

    [Header("Connected Objects"), SerializeField]
    protected Wall m_RightWall;
    [SerializeField]
    protected Wall m_LeftWall;
    [SerializeField]
    protected Wall m_TopWall;
    [SerializeField]
    protected Wall m_BottomWall;

    protected SpriteRenderer m_SpriteRenderer;

    // Use this for initialization
    protected override void OnGameStart()
    {
        if ((m_SpriteRenderer = GetComponent<SpriteRenderer>()) == null)
            Debug.LogError(name + " has no sprite renderer!");
        if (!m_HorizontalCenter || !m_HorizontalEdge || !m_VerticalCenter || !m_VerticalEdge)
            Debug.LogError(name + " is missing a sprite!");

        m_TileIndex = Controller.GetArrayIndex(transform.position, m_UnitGridSize, Vector3.zero);

        foreach (GameObject other in Controller.Tiles[m_TileIndex.ToString()])
            if (other && other != gameObject && other.GetComponent<Wall>())
                DestroyImmediate(other);

        if (Controller.Tiles.ContainsKey(new IntVector2(m_TileIndex.x + 1, m_TileIndex.y).ToString()))
            foreach (GameObject other in Controller.Tiles[new IntVector2(m_TileIndex.x + 1, m_TileIndex.y).ToString()])
                if (other && other != gameObject && other.GetComponent<Wall>())
                    m_RightWall = other.GetComponent<Wall>();

        if (Controller.Tiles.ContainsKey(new IntVector2(m_TileIndex.x - 1, m_TileIndex.y).ToString()))
            foreach (GameObject other in Controller.Tiles[new IntVector2(m_TileIndex.x - 1, m_TileIndex.y).ToString()])
                if (other && other != gameObject && other.GetComponent<Wall>())
                    m_LeftWall = other.GetComponent<Wall>();

        if (Controller.Tiles.ContainsKey(new IntVector2(m_TileIndex.x, m_TileIndex.y + 1).ToString()))
            foreach (GameObject other in Controller.Tiles[new IntVector2(m_TileIndex.x, m_TileIndex.y + 1).ToString()])
                if (other && other != gameObject && other.GetComponent<Wall>())
                    m_BottomWall = other.GetComponent<Wall>();

        if (Controller.Tiles.ContainsKey(new IntVector2(m_TileIndex.x, m_TileIndex.y - 1).ToString()))
            foreach (GameObject other in Controller.Tiles[new IntVector2(m_TileIndex.x, m_TileIndex.y - 1).ToString()])
                if (other && other != gameObject && other.GetComponent<Wall>())
                    m_TopWall = other.GetComponent<Wall>();

        UpdateSprite();
    }

    protected override void OnEditorUpdate()
    {

    }
    protected override void OnEditorUpdateSelected()
    {
        Controller.SnapToGrid(this);
    }
    // Update is called once per frame while the game is running
    protected override void OnGameUpdate()
    {

    }

    protected virtual void UpdateSprite()
    {
        if (m_RightWall && m_LeftWall)
            m_SpriteRenderer.sprite = m_HorizontalCenter;
        if (m_TopWall && m_BottomWall)
            m_SpriteRenderer.sprite = m_VerticalCenter;

        if (!m_TopWall && m_BottomWall)
            m_SpriteRenderer.sprite = m_VerticalEdge;
        if (m_TopWall && !m_BottomWall)
            m_SpriteRenderer.sprite = m_HorizontalEdge;
    }

    protected virtual void OnDestroy()
    {
        //Controller.RemoveTile(this);
    }
}
