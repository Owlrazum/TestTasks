using UnityEngine;
using UnityEngine.Assertions;
using TMPro;

[RequireComponent(typeof(CanvasGroup))]
public class RoomPlayerUI : MonoBehaviour
{
    [SerializeField]
    private float _hideAlphaValue = 0.25f;

    [Header("References")]
    [SerializeField]
    private TextMeshProUGUI _localPlayerName;

    [SerializeField]
    private TextMeshProUGUI _otherPlayerName;

    [SerializeField]
    private Button _readyStateButton;

    [SerializeField]
    private NameInputField _localNameInputField;

    private CanvasGroup _canvasGroup;

    private const string ReadyText = "Ready";
    private const string NotReadyText = "NotReady";

    private bool _isLocal;
    private bool _isLocalReady;

    private void Awake()
    {
        _localPlayerName.transform.parent.gameObject.SetActive(false); // parent because of background
        _otherPlayerName.transform.parent.gameObject.SetActive(false);
        _readyStateButton.CanInteract = false;
        _localNameInputField.DeactivateInputField();

        TryGetComponent(out _canvasGroup);

        _canvasGroup.alpha = _hideAlphaValue;
    }

    private void OnLocalNameChanged(string newName)
    {
        _localPlayerName.text = newName;
    }

    public void Show(bool isLocal, string playerName)
    {
        _isLocal = isLocal;
        _canvasGroup.alpha = 1;

        if (isLocal)
        {
            _localPlayerName.transform.parent.gameObject.SetActive(true);
            _otherPlayerName.transform.parent.gameObject.SetActive(false);
            _readyStateButton.CanInteract = true;
            _readyStateButton.OnClick += OnLocalPlayerReadyButtonClicked;
            
            _localNameInputField.Activate();
            _localNameInputField.EventOnValueChanged += OnLocalNameChanged;
            _localPlayerName.text = playerName;
        }
        else
        {
            _otherPlayerName.transform.parent.gameObject.SetActive(true);
            _localPlayerName.transform.parent.gameObject.SetActive(false);
            _localNameInputField.gameObject.SetActive(false);

            Assert.IsNotNull(playerName);
            _otherPlayerName.text = playerName;
        }
    }

    private void OnDestroy()
    { 
        if (_isLocal)
        {
            _readyStateButton.OnClick -= OnLocalPlayerReadyButtonClicked;
            _localNameInputField.EventOnValueChanged -= OnLocalNameChanged;
        }
    }

    public void Hide()
    {
        _canvasGroup.alpha = _hideAlphaValue;
        Debug.Log("hh");
        _localPlayerName.transform.parent.gameObject.SetActive(false);
        _otherPlayerName.transform.parent.gameObject.SetActive(true);

        _localNameInputField.DeactivateInputField();
        _localNameInputField.gameObject.SetActive(false);
    }

    private void OnLocalPlayerReadyButtonClicked()
    {
        _isLocalReady = !_isLocalReady;
        _readyStateButton.ChangeButtonText(_isLocalReady ? ReadyText : NotReadyText);
        
        string playerName = _localNameInputField.GetPlayerName();
        _localPlayerName.text = playerName;

        if (_isLocalReady)
        {
            _localNameInputField.DeactivateInputField();
            _localNameInputField.gameObject.SetActive(false);
        }
        else
        { 
            _localNameInputField.Activate();
            _localNameInputField.gameObject.SetActive(true);   
        }

        NetworkRoom.EventLocalReadyStatusChange(_isLocalReady, playerName);
    }

    public void OnOtherPlayerReadyStatusChanged(bool readyStatus, string playerName = null)
    {
        _readyStateButton.ChangeButtonText(readyStatus ? ReadyText : NotReadyText);
        if (playerName != null)
        {
            _otherPlayerName.text = playerName;
        }
    }
}
