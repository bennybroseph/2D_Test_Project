using UnityEngine;
using System.Collections.Generic;

namespace TileMapper
{
    public class TilemapController : ScriptableObject
    {
        [SerializeField]
        protected List<Tile> m_Tiles;

        // Use this for initialization
        void Start()
        {
            m_Tiles = new List<Tile>();
            m_Tiles.AddRange(FindObjectsOfType<Tile>());
        }

        // Update is called once per frame
        void Update()
        {

        }
    }
}