using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

namespace TileMapperEditor
{
    [CustomEditor(typeof(TileMapper.Controller))]
    public class LevelScriptEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();
            TileMapper.Controller myTarget = (TileMapper.Controller)target;
            
            foreach (List<TileMapper.Tile> Tiles in myTarget.m_Tiles)
                foreach (TileMapper.Tile Tile in Tiles)
                    EditorGUILayout.TextField(Tile.name);
            //myTarget.m_Tiles = EditorGUILayout.IntField("Experience", myTarget.experience);
            //EditorGUILayout.LabelField("Level", myTarget.Level.ToString());
        }
    }
}