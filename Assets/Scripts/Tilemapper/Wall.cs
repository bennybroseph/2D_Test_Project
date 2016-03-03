using UnityEngine;
using System.Collections;
using TileMapper;

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

    protected SpriteRenderer m_SpriteRenderer;

    // Use this for initialization
    protected override void OnGameStart()
    {
        if ((m_SpriteRenderer = GetComponent<SpriteRenderer>()) == null)
            Debug.LogError(name + " has no sprite renderer!");
        if (m_HorizontalCenter == null || m_HorizontalEdge == null || m_VerticalCenter == null || m_VerticalEdge == null)
            Debug.LogError(name + " is missing a sprite!");
    }

    protected override void OnEditorUpdate()
    {
        //m_TileIndex = Controller.ConvertToIndex(transform.position);

        //foreach (GameObject other in Controller.Tiles[(int)m_TileIndex.x, (int)m_TileIndex.y])
        //{
        //    if (other != null &&other != gameObject && other.GetComponent<Wall>() != null)
        //        DestroyImmediate(other);
        //}
    }
    protected override void OnEditorUpdateSelected()
    {
        Controller.SnapToGrid(this);
    }
    // Update is called once per frame while the game is running
    protected override void OnGameUpdate()
    {

    }
    protected virtual void OnDestroy()
    {
        //Controller.RemoveTile(this);
    }
}
