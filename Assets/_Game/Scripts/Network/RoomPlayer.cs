using Mirror;
using UnityEngine;
using UnityEngine.Assertions;

public class RoomPlayer : NetworkRoomPlayer
{
    private RoomUI _roomUI;
    private bool _isLocalInitialized;
    
    private bool _isReady;

    public override void OnStartServer()
    {
        _isLocalInitialized = true; // after viewing source, it seems isLocalPlayer will be initialized
        _roomUI = UIDelegatesContainer.GetRoomUI();

        if (isLocalPlayer)
        { 
            UIDelegatesContainer.EventLocalReadyButtonClick += CmdChangeReadyState;
        }
    }

    private void OnDestroy()
    {
        if (isLocalPlayer)
        { 
            UIDelegatesContainer.EventLocalReadyButtonClick -= CmdChangeReadyState;
        }
    }

    public override void OnClientEnterRoom()
    {
        Assert.IsTrue(_isLocalInitialized);
        _roomUI.ShowPlayerAtIndex(index);
    }

    public override void ReadyStateChanged(bool oldReadyState, bool newReadyState)
    {
        if (!isLocalPlayer)
        { 
            _roomUI.OnOtherPlayerReadyStatusChanged(index, newReadyState);
        }
    }

    public override void OnGUI()
    {
        base.OnGUI();
        // using Unity UI
    }
}