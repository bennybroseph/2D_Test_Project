using UnityEngine;
using System.Collections;

public class Player : TileMapper.Tile
{
    [SerializeField]
    Rigidbody2D m_Rigidbody;

    protected override void OnGameStart()
    {
        m_Rigidbody = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    protected override void OnGameUpdate()
    {
        if (Input.GetAxisRaw("Horizontal") != 0)
            m_Rigidbody.velocity = new Vector2(Input.GetAxisRaw("Horizontal") * 0.5f, m_Rigidbody.velocity.y);
        else
            m_Rigidbody.velocity = new Vector2(0.0f, m_Rigidbody.velocity.y);

        if (Input.GetAxisRaw("Vertical") != 0)
            m_Rigidbody.velocity = new Vector2(m_Rigidbody.velocity.x,Input.GetAxisRaw("Vertical") * 0.5f);
        else
            m_Rigidbody.velocity = new Vector2(m_Rigidbody.velocity.x, 0.0f);
    }
}
