using System.Collections.Generic;
using System.IO;
using UnityEngine;

/**
 * The EventLog takes events as strings from other parts of the game
 * and prints them to the screen and to a text file.
 *
 * The EventLog handles the GUI side of printing to screen and also
 * the file IO side of printing to a file.
 * 
 * A new timestamped text file is open for each playthrough. 
 */
public class EventLog : MonoBehaviour
{
    // ====================================================
    // Data Members
    // ====================================================
    List<string> _eventList;

    Rect _labelRect;
    GUIStyle _labelStyle;
    public Font labelFont;
    private StreamWriter _writer;
    private bool _logFileEnabled = false;

    // ====================================================
    // Event Methods
    // ====================================================
    
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

        try
        {
            // Set up the log text file
            string path = Application.persistentDataPath + "/" + System.DateTime.Now.ToString() + " Log.txt";
            _writer = new StreamWriter(path);
            _logFileEnabled = true;
            LogEvent("File path for log is " + path);
            _writer.WriteLine("Created StreamWriter.");
        }
        catch (DirectoryNotFoundException e)
        {
            LogEvent("Failed to write to a log file.");
            _logFileEnabled = false;
        }

        LogEvent("Set up the EventLog.");
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

    // ====================================================
    // Other Methods
    // ====================================================

    /**
     * The main feature of EventLog is this method. Logs an event
     * to an ingame console, to a text file, and to the Unity
     * Engine logs.
     */
    public void LogEvent(string eventString)
    {
        // write to ingame event log
        _eventList.Add(eventString);
        
        // write to text file
        if (_logFileEnabled)
            _writer.WriteLine(eventString);
        
        Debug.Log("Logged event: " + eventString);
    }

    public void CloseWriter()
    {
        _writer.Close();
    }
    
    void OnDestroy()
    {
        _writer.Close();
    }
}
