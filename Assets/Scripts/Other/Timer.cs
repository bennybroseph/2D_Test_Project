using UnityEngine;
using System.Collections;

public class Timer : MonoBehaviour
{
    protected float m_TimeToWait;
    protected float m_CurrentDelay;
    protected float m_TimeCounted;

    // Use this for initialization
    void Start()
    {
        StartCoroutine("Count");
    }

    IEnumerator Count()
    {
        while (m_TimeCounted != 10)
        {
            m_CurrentDelay += Time.deltaTime;

            if (m_CurrentDelay >= 1)
            {
                m_CurrentDelay = 0;
                m_TimeCounted++;
                Debug.Log(m_TimeCounted);
            }
            yield return null;
        }
        Debug.Log("Done");
    }
    // Update is called once per frame
    void Update()
    {

    }
}
