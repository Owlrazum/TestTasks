using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
using Mirror;

public class RoomUI : NetworkBehaviour
{
    [SerializeField]
    private Transform _roomPlayersUIParent;
    private List<RoomPlayerUI> _roomPlayersUI;

    private void Awake()
    {
        UIDelegatesContainer.GetRoomUI += GetRoomUI;
        
        
        InitializeRoomPlayersUIList();
    }

    private void OnDestroy()
    {
        UIDelegatesContainer.GetRoomUI -= GetRoomUI;
    }

    private RoomUI GetRoomUI()
    {
        return this;
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

    public void ShowPlayerAtIndex(int index)
    {
        Show(index);
    }

    public void Hide(int index)
    {
        _roomPlayersUI[index].Hide();
    }

    public void Show(int index)
    {
        _roomPlayersUI[index].Show(isLocalPlayer);
    }

    public void OnOtherPlayerReadyStatusChanged(int index, bool readyStatus)
    {
        _roomPlayersUI[index].OnOtherPlayerReadyStatusChanged(readyStatus);
    }
}