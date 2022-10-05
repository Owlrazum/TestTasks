using UnityEngine;

public class OfflineUI : MonoBehaviour
{
    [SerializeField]
    private Button _startServerButton;

    [SerializeField]
    private Button _startHostButton;

    [SerializeField]
    private Button _startClientButton;

    private void Awake()
    {
        _startHostButton.OnClick += OnStartHostButtonClicked;
        _startClientButton.OnClick += OnStartClientButtonClicked;
        _startServerButton.OnClick += OnStartServerButtonClicked;
    }

    private void OnDestroy()
    { 
        _startHostButton.OnClick -= OnStartHostButtonClicked;
        _startClientButton.OnClick -= OnStartClientButtonClicked;
        _startServerButton.OnClick -= OnStartServerButtonClicked;
    }

    private void OnStartHostButtonClicked()
    {
        NetworkDelegatesContainer.StartHost();
    }

    private void OnStartClientButtonClicked()
    { 
        NetworkDelegatesContainer.StartClient();
    }

    private void OnStartServerButtonClicked()
    {
        NetworkDelegatesContainer.StartServer();
    }
}
