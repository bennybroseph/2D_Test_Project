using UnityEngine;
using UnityEngine.UI;
using System.IO;
using System.Collections;

public class TextScript : MonoBehaviour
{
    [SerializeField]
    Text m_Text;
    [SerializeField]
    bool m_Bool;

    // Use this for initialization
    void Start()
    {
        m_Text = GetComponent<Text>();
        m_Text.text = "";

        StreamReader OpenFile;
        if (m_Bool)
            OpenFile = new StreamReader("Test.txt");
        else
            OpenFile = new StreamReader("Shrug.txt");

        string line;
        while ((line = OpenFile.ReadLine()) != null)
            m_Text.text += line + "\n";

        OpenFile.Close();
    }

    // Update is called once per frame
    void Update()
    {

    }
}
