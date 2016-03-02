using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class DebugText : MonoBehaviour
{
    private Text m_Text;
    // Use this for initialization
    void Start()
    {
        m_Text = GetComponent<Text>();

    }

    // Update is called once per frame
    void Update()
    {
        Vector3 MousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        

        MousePos.x *= 100.0f;
        MousePos.x = (int)((MousePos.x + ((MousePos.x > 0) ? 8 : -8)) / 16) * 16;
        MousePos.y *= 100.0f;
        MousePos.y = (int)((MousePos.y + ((MousePos.y > 0) ? 8 : -8)) / 16) * 16;
        m_Text.text = MousePos.ToString();
    }
}
