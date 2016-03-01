using UnityEngine;
using System.Collections;

public class Wall : TileMapper.Tile
{
    [SerializeField]
    Sprite m_HorizontalCenter;
    [SerializeField]
    Sprite m_HorizontalEdge;
    [SerializeField]
    Sprite m_VerticalCenter;
    [SerializeField]
    Sprite m_VerticalEdge;

    // Use this for initialization
    protected override void OnGameStart()
    {
        if (m_HorizontalCenter == null || m_HorizontalEdge == null || m_VerticalCenter == null || m_VerticalEdge == null)
            Debug.LogError(name + " is missing a sprite!");
    }

    protected override void OnEditorUpdateSelected()
    {
        TileMapper.Controller.SnapToGrid(this);
    }
    // Update is called once per frame
    protected override void OnGameUpdate()
    {

    }
}
