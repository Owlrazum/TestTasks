using System;
using UnityEngine;
using UnityEngine.Events;
using TMPro;

[RequireComponent(typeof(TMP_InputField))]
public class NameInputField : MonoBehaviour
{
    private TMP_InputField _inputField;
    private bool _isNameGiven;
    private UnityAction<string> OnValueChangedAction;
    private Player _player;

    public Action<string> EventOnValueChanged;
    private void Awake()
    {
        TryGetComponent(out _inputField);
        OnValueChangedAction += OnValueChanged;
        _inputField.onValueChanged.AddListener(OnValueChangedAction);
    }

    private void OnDestroy()
    {
        OnValueChangedAction -= OnValueChanged;
        _inputField.onValueChanged.RemoveAllListeners();
    }

    public void Activate()
    {
        _inputField.ActivateInputField();
    }

    public void DeactivateInputField()
    { 
        _inputField.DeactivateInputField();
    }

    private void OnValueChanged(string newName)
    {
        _isNameGiven = true;
        EventOnValueChanged?.Invoke(newName);
    }

    public string GetPlayerName()
    { 
        if (_isNameGiven)
        { 
            return _inputField.text;
        }
        else
        {
            return null;
        }
    }
}
