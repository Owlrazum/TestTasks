using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

public class RoomUI : MonoBehaviour
{
    [SerializeField]
    private Transform _roomPlayersUIParent;
    private List<RoomPlayerUI> _roomPlayersUI;

    private void Awake()
    {
        UIDelegatesContainer.GetRoomUI += GetRoomUI;

        NetworkDelegatesContainer.EventLocalPlayerAssignedIndex += ShowLocalPlayerSlot;

        NetworkDelegatesContainer.EventOtherPlayerRegisteredInRoom += ShowOtherPlayerSlot;
        NetworkDelegatesContainer.EventOtherPlayerUnregisteredInRoom += HideOtherPlayerSlot;
        NetworkDelegatesContainer.EventOtherClientReadyStatusChanged += OnOtherPlayerReadyStatusChanged;

        InitializeRoomPlayersUIList();
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
        UIDelegatesContainer.GetRoomUI -= GetRoomUI;

        NetworkDelegatesContainer.EventLocalPlayerAssignedIndex -= ShowLocalPlayerSlot;

        NetworkDelegatesContainer.EventOtherPlayerRegisteredInRoom -= ShowOtherPlayerSlot;
        NetworkDelegatesContainer.EventOtherPlayerUnregisteredInRoom -= HideOtherPlayerSlot;
        NetworkDelegatesContainer.EventOtherClientReadyStatusChanged -= OnOtherPlayerReadyStatusChanged;
    }

    private RoomUI GetRoomUI()
    {
        return this;
    }

    public void ShowLocalPlayerSlot(int index)
    { 
        _roomPlayersUI[index].Show(true);
    }

    public void ShowOtherPlayerSlot(int index)
    {
        _roomPlayersUI[index].Show(false);
    }

    public void HideOtherPlayerSlot(int index)
    {
        _roomPlayersUI[index].Hide();
    }

    public void OnOtherPlayerReadyStatusChanged(int index, bool readyStatus)
    {
        _roomPlayersUI[index].OnOtherPlayerReadyStatusChanged(readyStatus);
    }
}