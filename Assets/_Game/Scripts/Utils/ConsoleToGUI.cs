using UnityEngine;
using TMPro;

public class ConsoleToGUI : MonoBehaviour
{
    [SerializeField]
    private GameObject _errorLog;

    [SerializeField]
    private TextMeshProUGUI _logMesh;

    [SerializeField]
    private TextMeshProUGUI _errorMesh;

    void OnEnable()
    {
        DontDestroyOnLoad(gameObject);
        // _errorLog.SetActive(false);
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
            if (_errorMesh.text.Length > 400)
            {
                _errorMesh.text = "";
            }
            _errorMesh.text += logString + "\n" + stackTrace + "\n";
            return;
        }

        if (_logMesh.text.Length > 400)
        {
            _logMesh.text = "";
        }
        _logMesh.text += logString + "\n";
    }
}