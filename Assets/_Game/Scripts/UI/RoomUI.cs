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
    private Dictionary<Player, int> _playerToRoomPlayerUI;

    private void Awake()
    {
        NetworkRoom.EventPlayerRegistered += ShowPlayerSlot;
        // NetworkRoom.EventOtherPlayerUnregisteredInRoom += HideOtherPlayerSlot;

        NetworkRoom.ActionShowStartButton += ShowStartButton;
        NetworkRoom.ActionHideStartButton += HideStartButton;
        NetworkRoom.ActionAllowStartButton += AllowStartButton;
        NetworkRoom.ActionDenyStartButton += DenyStartButton;

        InitializeContainers();
    }

    private void OnDestroy()
    {
        NetworkRoom.EventPlayerRegistered -= ShowPlayerSlot;
        // NetworkRoom.EventOtherPlayerUnregisteredInRoom -= HideOtherPlayerSlot;

        NetworkRoom.ActionShowStartButton -= ShowStartButton;
        NetworkRoom.ActionHideStartButton -= HideStartButton;
        NetworkRoom.ActionAllowStartButton -= AllowStartButton;
        NetworkRoom.ActionDenyStartButton -= DenyStartButton;

        if (_playerToRoomPlayerUI.Count > 0)
        {
            foreach (var kv in _playerToRoomPlayerUI)
            {
                Player player = kv.Key;
                player.EventIndexChanged -= OnPlayerIndexChanged;
            }
        }
    }

    private void InitializeContainers()
    {
        _roomPlayersUI = new List<RoomPlayerUI>(_roomPlayersUIParent.childCount);
        for (int i = 0; i < _roomPlayersUIParent.childCount; i++)
        {
            _roomPlayersUI.Add(_roomPlayersUIParent.GetChild(i).GetComponent<RoomPlayerUI>());
            Assert.IsNotNull(_roomPlayersUI[i]);
        }

        _playerToRoomPlayerUI = new Dictionary<Player, int>(4);
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

    private void ShowPlayerSlot(Player player)
    {
        _roomPlayersUI[player.Index].Show(player);
        player.EventIndexChanged += OnPlayerIndexChanged;
        _playerToRoomPlayerUI.Add(player, player.Index);
    }

    private void OnPlayerIndexChanged(Player movedPlayer, int oldIndex, int newIndex)
    {
        // _roomPlayersUI[oldIndex].Hide();
        // _roomPlayersUI[newIndex].Show(movedPlayer);
    }

    // private void HideOtherPlayerSlot(int index)
    // {
    //     _roomPlayersUI[index].Hide();
    // }

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