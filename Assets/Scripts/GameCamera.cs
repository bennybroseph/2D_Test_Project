using UnityEngine;
using System.Collections;

public class GameCamera : MonoBehaviour
{
    [SerializeField]
    GameObject m_Following;

    // Use this for initialization
    void Start()
    {
        if (m_Following == null)
            Debug.LogWarning("m_Following is null. Camera won't function...");
    }

    // Update is called once per frame
    void Update()
    {
        if(m_Following != null)
        {
            transform.position = new Vector3(m_Following.transform.position.x, m_Following.transform.position.y, transform.position.z);
        }
    }
}
