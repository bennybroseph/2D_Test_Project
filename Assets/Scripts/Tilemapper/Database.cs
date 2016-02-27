using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

// Has good examples of ScriptableObject functions, but was not useful to me at the time
namespace TileMapper
{
    //[InitializeOnLoad, ExecuteInEditMode]
    //public class Database : ScriptableObject
    //{
    //    [SerializeField]
    //    private List<Tile> m_Tiles;
    //    [SerializeField]
    //    private GameObject m_Controller;

    //    static Database s_Database;

    //    static Database()
    //    {
    //        if (s_Database == null)
    //            s_Database = CreateInstance<Database>();

    //        if (!System.IO.File.Exists("Assets/TileMapperController.asset"))
    //        {
    //            AssetDatabase.CreateAsset(s_Database, "Assets/TileMapperController.asset");
    //            AssetDatabase.SaveAssets();
    //        }            
    //    }

    //    static public void OnSceneUpdate()
    //    {
    //        //Debug.Log("Something changed");
    //    }

    //    void OnValidate()
    //    {
    //        if (FindObjectOfType<Controller>() == null)
    //        {
    //            s_Database.m_Controller = new GameObject("Tilemap Controller");
    //            s_Database.m_Controller.AddComponent<Controller>();
    //        }
    //    }
    //}    
}