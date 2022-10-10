using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

using Mirror;

public class NetworkRoom : NetworkBehaviour
{
    #region Publish
    public static Action NetworkRoomInitialized;

    public static Action<int> EventOtherPlayerRegisteredInRoom;
    public static Action<int> EventLocalPlayerAssignedIndex;
    public static Action<int> EventOtherPlayerUnregisteredInRoom;

    public static Action<int, bool> NotifyClientReadyStatusChange;
    public static Action<int, bool> EventOtherClientReadyStatusChanged;

    public static Action ActionShowStartButton;
    public static Action ActionHideStartButton;
    public static Action ActionAllowStartButton;
    public static Action ActionDenyStartButton;
    #endregion

    #region Subscribe
    public static Action<GameObject> ActionRegisterPlayerInRoom;
    public static Action<bool> EventLocalReadyStatusChange;
    public static Action EventStartGameButtonPress;
    #endregion

    [SerializeField]
    private int _minimumReadyPlayerCount = 2;

    private Dictionary<int, Player> _players;
    private Player _hostPlayer;

    private int _serverPlayerCount;
    private int _readyPlayerCount;

    private bool _isStartAllowed;

    private int _clientLocalPlayerIndex;

    public override void OnStartServer()
    {
        _players = new Dictionary<int, Player>();
        _serverPlayerCount = 0;
        _readyPlayerCount = 0;
    }

    public override void OnStartClient()
    {
        Debug.Log("NetworkRoom start client");
        ActionRegisterPlayerInRoom += RegisterPlayerInRoom;

        EventLocalReadyStatusChange += OnLocalReadyStatusChange;
        EventStartGameButtonPress += OnStartGameButtonPress;

        NetworkRoomInitialized?.Invoke();
    }

    public override void OnStopClient()
    {
        ActionRegisterPlayerInRoom -= RegisterPlayerInRoom;

        EventLocalReadyStatusChange -= OnLocalReadyStatusChange;
        EventStartGameButtonPress -= OnStartGameButtonPress;
    }

    #region Registration
    private void RegisterPlayerInRoom(GameObject player)
    {
        CmdRegisterPlayer(player);
    }
    [Command(requiresAuthority = false)]
    private void CmdRegisterPlayer(GameObject playerGb)
    {
        var newPlayer = playerGb.GetComponent<Player>();
        Assert.IsNotNull(newPlayer);
        int newIndex = _serverPlayerCount;
        newPlayer.Index = newIndex;
        _serverPlayerCount++;
        if (_serverPlayerCount == 1)
        {
            _hostPlayer = newPlayer;
            TargetShowStartButton(_hostPlayer.connectionToClient);
        }
        else
        {
            TargetHideStartButton(newPlayer.connectionToClient);
        }

        foreach (var keyValue in _players)
        {
            int playerIndex = keyValue.Key;
            NetworkConnectionToClient connection = keyValue.Value.connectionToClient;
            TargetOtherPlayerAdded(connection, newIndex);
            TargetOtherPlayerAdded(newPlayer.connectionToClient, playerIndex);
        }

        _players.Add(newIndex, newPlayer);
        TargetLocalPlayerAdded(newPlayer.connectionToClient, newIndex);
    }
    [TargetRpc]
    private void TargetOtherPlayerAdded(NetworkConnection connection, int playerIndex)
    {
        EventOtherPlayerRegisteredInRoom(playerIndex);
    }
    [TargetRpc]
    private void TargetLocalPlayerAdded(NetworkConnection connection, int playerIndex)
    {
        _clientLocalPlayerIndex = playerIndex;
        EventLocalPlayerAssignedIndex(playerIndex);
    }
    #endregion

    #region ReadyStatus
    private void OnLocalReadyStatusChange(bool newReadyStatus)
    {
        CmdClientReadyStatusChanged(_clientLocalPlayerIndex, newReadyStatus);
    }
    [Command(requiresAuthority = false)]
    private void CmdClientReadyStatusChanged(int clientIndex, bool newReadyStatus)
    {
        if (newReadyStatus)
        {
            _readyPlayerCount++;
        }
        else
        {
            _readyPlayerCount--;
        }

        Debug.Log($"_readyPlayerCount {_readyPlayerCount} {_minimumReadyPlayerCount} {_isStartAllowed}");
        if (_readyPlayerCount >= _minimumReadyPlayerCount && !_isStartAllowed)
        {
            _isStartAllowed = true;
            TargetAllowStartButton(_hostPlayer.connectionToClient);
        }
        else if (_readyPlayerCount < _minimumReadyPlayerCount && _isStartAllowed)
        {
            _isStartAllowed = false;
            TargetDenyStartButton(_hostPlayer.connectionToClient);
        }

        RpcClientReadyStatusChanged(clientIndex, newReadyStatus);
    }
    [ClientRpc(includeOwner = false)]
    private void RpcClientReadyStatusChanged(int clientIndex, bool readyStatus)
    {
        EventOtherClientReadyStatusChanged?.Invoke(clientIndex, readyStatus);
    }
    [TargetRpc]
    private void TargetShowStartButton(NetworkConnection connection)
    {
        ActionShowStartButton();
    }
    [TargetRpc]
    private void TargetHideStartButton(NetworkConnection connection)
    {
        ActionHideStartButton();
    }
    [TargetRpc]
    private void TargetAllowStartButton(NetworkConnection connection)
    {
        ActionAllowStartButton();
    }
    [TargetRpc]
    private void TargetDenyStartButton(NetworkConnection connection)
    {
        ActionDenyStartButton();
    }

    #endregion

    private void OnStartGameButtonPress()
    {
        CmdStartGame();
    }
    [Command(requiresAuthority = false)]
    private void CmdStartGame()
    {
        GameController.ActionServerGameStart(_players);
    }
}
