using UnityEngine;
using UnityEngine.Assertions;

[RequireComponent(typeof(CanvasGroup))]
public class RoomPlayerUI : MonoBehaviour
{
    [SerializeField]
    private float _hideAlphaValue = 0.25f;

    private Transform _localPlayerUI;
    private Transform _otherPlayerUI;
    private Button _readyStateButton;
    private CanvasGroup _canvasGroup;

    private const string ReadyText = "Ready";
    private const string NotReadyText = "NotReady";

    private bool _isLocal;
    private bool _isLocalReady;

    private void Awake()
    {
        _localPlayerUI = transform.GetChild(0);
        _otherPlayerUI = transform.GetChild(1);

        _localPlayerUI.gameObject.SetActive(false);
        _otherPlayerUI.gameObject.SetActive(false);

        _readyStateButton = transform.GetChild(2).GetComponent<Button>();

        Assert.IsTrue(_localPlayerUI.name == "LocalPlayer" && _otherPlayerUI.name == "OtherPlayer" && _readyStateButton != null,
            "RoomPlayerUI assumes the order of its children to be first local, second other playerUI, third ready state button");

        TryGetComponent(out _canvasGroup);

        _readyStateButton.CanInteract = false;
        _canvasGroup.alpha = _hideAlphaValue;
    }

    public void Show(bool isLocal)
    {
        _isLocal = isLocal;
        _canvasGroup.alpha = 1;

        if (isLocal)
        {
            _localPlayerUI.gameObject.SetActive(true);
            _otherPlayerUI.gameObject.SetActive(false);
            _readyStateButton.CanInteract = true;
            _readyStateButton.OnClick += OnLocalPlayerReadyButtonClicked;
        }
        else
        {
            _otherPlayerUI.gameObject.SetActive(true);
            _localPlayerUI.gameObject.SetActive(false);
        }
    }

    public void Hide()
    {
        _canvasGroup.alpha = _hideAlphaValue;
        Debug.Log("hh");
        _localPlayerUI.gameObject.SetActive(false);
        _otherPlayerUI.gameObject.SetActive(true);
    }

    private void OnDestroy()
    {
        if (_isLocal)
        {
            _readyStateButton.OnClick -= OnLocalPlayerReadyButtonClicked;
        }
    }

    private void OnLocalPlayerReadyButtonClicked()
    {
        _isLocalReady = !_isLocalReady;
        _readyStateButton.ChangeButtonText(_isLocalReady ? ReadyText : NotReadyText);
        UIDelegatesContainer.EventLocalReadyStatusChange(_isLocalReady);
    }

    public void OnOtherPlayerReadyStatusChanged(bool readyStatus)
    {
        _readyStateButton.ChangeButtonText(readyStatus ? ReadyText : NotReadyText);
    }
}
