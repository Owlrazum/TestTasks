using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Settings : MonoBehaviour
{
    public static Settings Singleton;
    private void Awake()
    {
        if (Singleton == null)
        {
            Singleton = this;
            mode = InputMode.MouseKeyboard;
            //Cursor.visible = true;
            //Cursor.lockState = CursorLockMode.Confined;
        }
    }

    [SerializeField]
    private Vector2 size;
    public Vector2 Size { get { return size; } }

    public enum InputMode
    { 
        Keyboard,
        MouseKeyboard
    }

    [HideInInspector]
    public InputMode mode;
    public void SwitchInputMode()
    {
        if (mode == InputMode.Keyboard)
        {
            mode = InputMode.MouseKeyboard;
        }
        else
        {
            mode = InputMode.Keyboard;
        }
    }
}
