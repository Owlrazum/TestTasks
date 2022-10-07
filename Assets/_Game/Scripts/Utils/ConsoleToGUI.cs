using UnityEngine;

// Copied from Internet and slightly modified
public class ConsoleToGUI : MonoBehaviour
{
    [SerializeField]
    private int _fontSize;
    //#if !UNITY_EDITOR
    static string myLog = "";
    private string output;
    private string stack;

    static string errorLog = "";

    private GUIStyle _textAreaStyle;
    private GUIStyle _textAreaErrorStyle;

    void OnEnable()
    {
        DontDestroyOnLoad(gameObject);
        _textAreaStyle = new GUIStyle("textArea");
        _textAreaStyle.fontSize = _fontSize;
        _textAreaErrorStyle = new GUIStyle("textArea");
        _textAreaErrorStyle.fontSize = _fontSize;
        _textAreaErrorStyle.fontStyle = FontStyle.Bold;
        Application.logMessageReceived += Log;
    }

    void OnDisable()
    {
        Application.logMessageReceived -= Log;
    }

    public void Log(string logString, string stackTrace, LogType type)
    {
        if (type == LogType.Error)
        {
            errorLog += logString;
        }
        output = logString;
        stack = stackTrace;
        myLog = output + "\n" + myLog;
        if (myLog.Length > 5000)
        {
            myLog = myLog.Substring(0, 4000);
        }
    }

    void OnGUI()
    {
        //if (!Application.isEditor) //Do not display in editor ( or you can use the UNITY_EDITOR macro to also disable the rest)
        {
            myLog = GUI.TextArea(new Rect(10, 10, 800, 100), myLog, _textAreaStyle);
            if (errorLog.Length > 0)
            {
                errorLog = GUI.TextArea(new Rect(10, 120, 800, 100), errorLog, _textAreaErrorStyle);
            }
        }
    }
    //#endif
}