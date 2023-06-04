using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventLog : MonoBehaviour
{
    // data members
    List<string> _eventList;

    Rect _labelRect;
    GUIStyle _labelStyle;
    public Font labelFont;

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

        _labelRect = new Rect(10, Screen.height - 90, 180, 320);
        _labelStyle = new GUIStyle();
        _labelStyle.font = labelFont;
        _labelStyle.fontSize = 16;
        _labelStyle.normal.textColor = Color.white;
        _labelStyle.normal.background = MakeTex( 2, 2, new Color( 0f, 0f, 0f, 0.5f ) );


        logEvent("Set up the EventLog.");
    }

    public void logEvent(string eventString)
    {
        _eventList.Add(eventString);
    }

    // Update is called once per frame
    void OnGUI()
    {
        string fullReport = "";
        
        if (_eventList.Count <= 5)
        {
            foreach (string eventString in _eventList)
            {
                fullReport += eventString + "\n";
            }
        }
        else
        {
            for (int i = 5; i >= 1; i--)
            {
                fullReport += _eventList[_eventList.Count - i] + "\n";
            }
        }

        GUI.Box(_labelRect, fullReport, _labelStyle);
    }
}
