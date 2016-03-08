using UnityEngine;
using System.Collections;
using Bennybroseph.System;

public class SystemTesting : MonoBehaviour
{
    Vector2<int> m_TestVector;
    // Use this for initialization
    void Start()
    {
        m_TestVector = new Vector2<int>(5, 5);
        Vector2<int> Test = new Vector2<int>(5, 5);

        Debug.Log(m_TestVector == Test);
    }

    // Update is called once per frame
    void Update()
    {

    }
}
