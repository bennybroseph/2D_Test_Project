using UnityEngine;
using UnityEditor;
using System.Collections;

//namespace TileMapperEditor
//{
//    [InitializeOnLoad]
//    public class Controller : MonoBehaviour
//    {
//        static TileMapper.Controller m_Controller;

//        static Controller()
//        {
//            // Ensures that a Controller object exists in the scene
//            if (FindObjectOfType<TileMapper.Controller>() == null)
//            {
//                GameObject NewController = new GameObject("Tilemap Controller");
//                NewController.AddComponent<TileMapper.Controller>();
//            }
//            m_Controller = FindObjectOfType<TileMapper.Controller>();

//            SceneView.onSceneGUIDelegate += OnSceneGUI;
//        }

//        static void OnSceneGUI(SceneView sceneView)
//        {
//            if (m_Controller.ShowGrid)
//            {
//                Vector3 CameraPos = Camera.current.transform.position;

//                Handles.color = m_Controller.GridColor;

//                Vector3 ScaledGridSize = m_Controller.GridSize;
//                if (CameraPos.z < -20)
//                    ScaledGridSize *= 2 * Mathf.Floor(CameraPos.z / -20);

//                Vector3 Start = new Vector3(
//                    Mathf.Round((CameraPos.x - Mathf.Abs(CameraPos.z) / 1.75f) * 100.0f / ScaledGridSize.x) * ScaledGridSize.x / 100.0f + m_Controller.GridSize.x / 200.0f,
//                    Mathf.Round((CameraPos.y - Mathf.Abs(CameraPos.z) / 1.75f) * 100.0f / ScaledGridSize.y) * ScaledGridSize.y / 100.0f + m_Controller.GridSize.y / 200.0f, 0.0f);
//                Vector3 End = new Vector3(
//                    Mathf.Round((CameraPos.x + Mathf.Abs(CameraPos.z) / 1.75f) * 100.0f / ScaledGridSize.x) * ScaledGridSize.x / 100.0f + m_Controller.GridSize.x / 200.0f,
//                    Mathf.Round((CameraPos.y + Mathf.Abs(CameraPos.z) / 1.75f) * 100.0f / ScaledGridSize.y) * ScaledGridSize.y / 100.0f + m_Controller.GridSize.y / 200.0f, 0.0f);

//                for (int i = 0; i < 5 + Mathf.Abs(CameraPos.z) * 6; ++i)
//                    Handles.DrawLine(new Vector3(Start.x + i * ScaledGridSize.x / 100.0f, Start.y, Start.z), new Vector3(Start.x + i * ScaledGridSize.x / 100.0f, End.y, Start.z));

//                for (int i = 0; i < 5 + Mathf.Abs(CameraPos.z) * 6; ++i)
//                    Handles.DrawLine(new Vector3(Start.x, Start.y + i * ScaledGridSize.y / 100.0f, Start.z), new Vector3(End.x, Start.y + i * ScaledGridSize.y / 100.0f, Start.z));
//            }
//        }
//    }
//}
