using UnityEngine;
using UnityEngine.Events;
using TMPro;

[RequireComponent(typeof(TMP_InputField))]
public class AddressInputField : MonoBehaviour
{
    [SerializeField]
    private Button _startClientButton;

    private TMP_InputField _inputField;
    private bool _isAddressGiven;
    private UnityAction<string> OnValueChangedAction;

    private void Awake()
    {
        TryGetComponent(out _inputField);
        OnValueChangedAction += OnValueChanged;
        _inputField.onValueChanged.AddListener(OnValueChangedAction);

        _startClientButton.BeforeOnClick += SetAddress;
    }

    private void OnDestroy()
    {
        _startClientButton.BeforeOnClick -= SetAddress;
    }

    private void OnValueChanged(string notUsed)
    {
        _isAddressGiven = true;
    }

    private void SetAddress()
    {
        if (_isAddressGiven)
        { 
            NetworkDelegatesContainer.UpdateNetworkAddress(_inputField.text);
        }
        else
        { 
            NetworkDelegatesContainer.UpdateNetworkAddress("localHost");
        }
    }
}
