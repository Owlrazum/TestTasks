using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

public class RoomUI : MonoBehaviour
{
    [SerializeField]
    private Transform _roomPlayersUIParent;

    [SerializeField]
    private Button _startButton;
    private List<RoomPlayerUI> _roomPlayersUI;

    private void Awake()
    {
        NetworkRoom.EventLocalPlayerAssignedIndex += ShowLocalPlayerSlot;
        NetworkRoom.EventOtherPlayerRegisteredInRoom += ShowOtherPlayerSlot;
        NetworkRoom.EventOtherPlayerUnregisteredInRoom += HideOtherPlayerSlot;
        NetworkRoom.EventOtherClientReadyStatusChanged += OnOtherPlayerReadyStatusChanged;

        NetworkRoom.ActionShowStartButton += ShowStartButton;
        NetworkRoom.ActionHideStartButton += HideStartButton;
        NetworkRoom.ActionAllowStartButton += AllowStartButton;
        NetworkRoom.ActionDenyStartButton += DenyStartButton;

        InitializeRoomPlayersUIList();
    }

    private void Start()
    {
        _startButton.CanInteract = false;
        _startButton.OnClick += OnStartButtonClicked;
    }

    private void OnStartButtonClicked()
    {
        NetworkRoom.EventStartGameButtonPress();
    }

    private void InitializeRoomPlayersUIList()
    {
        _roomPlayersUI = new List<RoomPlayerUI>(_roomPlayersUIParent.childCount);
        for (int i = 0; i < _roomPlayersUIParent.childCount; i++)
        {
            _roomPlayersUI.Add(_roomPlayersUIParent.GetChild(i).GetComponent<RoomPlayerUI>());
            Assert.IsNotNull(_roomPlayersUI[i]);
        }
    }

    private void OnDestroy()
    {
        NetworkRoom.EventLocalPlayerAssignedIndex -= ShowLocalPlayerSlot;
        NetworkRoom.EventOtherPlayerRegisteredInRoom -= ShowOtherPlayerSlot;
        NetworkRoom.EventOtherPlayerUnregisteredInRoom -= HideOtherPlayerSlot;
        NetworkRoom.EventOtherClientReadyStatusChanged -= OnOtherPlayerReadyStatusChanged;

        NetworkRoom.ActionShowStartButton -= ShowStartButton;
        NetworkRoom.ActionHideStartButton -= HideStartButton;
        NetworkRoom.ActionAllowStartButton -= AllowStartButton;
        NetworkRoom.ActionDenyStartButton -= DenyStartButton;
    }

    private void ShowLocalPlayerSlot(int index, string playerName)
    {
        _roomPlayersUI[index].Show(true, playerName);
    }

    private void ShowOtherPlayerSlot(int index, string playerName)
    {
        _roomPlayersUI[index].Show(false, playerName);
    }

    private void HideOtherPlayerSlot(int index)
    {
        _roomPlayersUI[index].Hide();
    }

    private void OnOtherPlayerReadyStatusChanged(int index, bool readyStatus, string playerName)
    {
        _roomPlayersUI[index].OnOtherPlayerReadyStatusChanged(readyStatus, playerName);
    }

    private void ShowStartButton()
    {
        _startButton.Show();
    }

    private void HideStartButton()
    {
        _startButton.Hide();
    }

    private void AllowStartButton()
    {
        _startButton.CanInteract = true;
    }

    private void DenyStartButton()
    {
        _startButton.CanInteract = false;
    }
}