using UnityEngine;
using System.Collections;
using Bennybroseph.MySystem;

public class SystemTesting : MonoBehaviour
{
    IntVector2 m_TestVector;
    // Use this for initialization
    void Start()
    {
        m_TestVector = new IntVector2(5, 5);
        IntVector2 Test = new IntVector2(5, 5);

        Debug.Log(m_TestVector == Test);
    }

    // Update is called once per frame
    void Update()
    {

    }
}
