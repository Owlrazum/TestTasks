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

    private Player _playerToShow;

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

    public void Show(Player player)
    {
        _playerToShow = player;
        _canvasGroup.alpha = 1;

        _playerToShow.EventReadyStatusChanged += OnPlayerToShowReadyStatusChange;
        _playerToShow.EventNameChanged += OnPlayerToShowNameChange;

        if (_playerToShow.IsLocalPlayer)
        {
            _localPlayerName.transform.parent.gameObject.SetActive(true);
            _otherPlayerName.transform.parent.gameObject.SetActive(false);
            _readyStateButton.CanInteract = true;
            _readyStateButton.OnClick += OnLocalPlayerReadyButtonClicked;

            _localNameInputField.Activate();
            _localNameInputField.EventOnValueChanged += OnLocalNameChanged;
            _localPlayerName.text = _playerToShow.Name;
        }
        else
        {
            _otherPlayerName.transform.parent.gameObject.SetActive(true);
            _localPlayerName.transform.parent.gameObject.SetActive(false);
            _localNameInputField.gameObject.SetActive(false);

            _otherPlayerName.text = _playerToShow.Name;
        }
    }

    private void OnDestroy()
    {
        if (_playerToShow == null)
        {
            return;
        }

        UnsubscribeFromEvents();
    }

    public void Hide()
    {
        _canvasGroup.alpha = _hideAlphaValue;
        _localPlayerName.transform.parent.gameObject.SetActive(false);
        _otherPlayerName.transform.parent.gameObject.SetActive(false);

        _localNameInputField.DeactivateInputField();
        _localNameInputField.gameObject.SetActive(false);

        UnsubscribeFromEvents();
        _playerToShow = null;
    }

    private void UnsubscribeFromEvents()
    {
        Assert.IsNotNull(_playerToShow, "No _playerToShow to hide!");
        _playerToShow.EventReadyStatusChanged -= OnPlayerToShowReadyStatusChange;
        _playerToShow.EventNameChanged -= OnPlayerToShowNameChange;

        if (_playerToShow.IsLocalPlayer)
        {
            _readyStateButton.OnClick -= OnLocalPlayerReadyButtonClicked;
            _localNameInputField.EventOnValueChanged -= OnLocalNameChanged;
        }
    }

    private void OnLocalPlayerReadyButtonClicked()
    {
        NetworkRoom.ActionChangeLocalPlayerReadyStatus();
        string playerName = _localNameInputField.GetPlayerName();
        if (playerName != null)
        {
            NetworkRoom.ActionChangeLocalPlayerName(playerName);
        }
    }

    private void OnPlayerToShowReadyStatusChange(bool newReadyStatus)
    {
        if (newReadyStatus)
        {
            _readyStateButton.ChangeButtonText(ReadyText);
            if (_playerToShow.IsLocalPlayer)
            {
                _localNameInputField.DeactivateInputField();
                _localNameInputField.gameObject.SetActive(false);
            }
        }
        else
        {
            _readyStateButton.ChangeButtonText(NotReadyText);
            if (_playerToShow.IsLocalPlayer)
            {
                _localNameInputField.Activate();
                _localNameInputField.gameObject.SetActive(true);
            }
        }
    }

    private void OnPlayerToShowNameChange(string newName)
    {
        if (_playerToShow == null)
        {
            return;
        }
        if (_playerToShow.IsLocalPlayer)
        {
            _localPlayerName.text = newName;
        }
        else
        {
            _otherPlayerName.text = newName;
        }
    }
}
