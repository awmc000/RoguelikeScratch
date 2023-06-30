using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class EventLog : MonoBehaviour
{
    // data members
    List<string> _eventList;

    Rect _labelRect;
    GUIStyle _labelStyle;
    public Font labelFont;
    private StreamWriter _writer;

    private Texture2D MakeTex( int width, int height, Color col )
    {
        Color[] pix = new Color[width * height];
        for( int i = 0; i < pix.Length; ++i )
        {
            pix[ i ] = col;
        }
        Texture2D result = new Texture2D( width, height );
        result.SetPixels( pix );
        result.Apply();
        return result;
    }

    // Start is called before the first frame update
    void Start()
    {
        _eventList = new List<string>();

        _labelRect = new Rect(10, Screen.height - 180, 180, 480);
        _labelStyle = new GUIStyle();
        _labelStyle.font = labelFont;
        _labelStyle.fontSize = 16;
        _labelStyle.normal.textColor = Color.white;
        _labelStyle.normal.background = MakeTex( 2, 2, new Color( 0f, 0f, 0f, 0.5f ) );

        // Set up the log text file
        string path = Application.persistentDataPath + "/" + System.DateTime.Now.ToString() + " Log";
        _writer = new StreamWriter(path);
        LogEvent("File path for log is " + path);
        _writer.WriteLine("Created StreamWriter.");
        
        LogEvent("Set up the EventLog.");
    }

    public void LogEvent(string eventString)
    {
        // write to ingame event log
        _eventList.Add(eventString);
        
        // write to text file
        _writer.WriteLine(eventString);
        
        Debug.Log("Logged event: " + eventString);
    }

    public void CloseWriter()
    {
        _writer.Close();
    }

    // Update is called once per frame
    void OnGUI()
    {
        string fullReport = "";
        
        if (_eventList.Count <= 10)
        {
            foreach (string eventString in _eventList)
            {
                fullReport += eventString + "\n";
            }
        }
        else
        {
            for (int i = 10; i >= 1; i--)
            {
                fullReport += _eventList[_eventList.Count - i] + "\n";
            }
        }

        GUI.Box(_labelRect, fullReport, _labelStyle);
    }

    void OnDestroy()
    {
        _writer.Close();
    }
}
