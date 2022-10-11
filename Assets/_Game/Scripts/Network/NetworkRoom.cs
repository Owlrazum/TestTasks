using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

using Mirror;

public class NetworkRoom : NetworkBehaviour
{
    #region Publish
    public static Action<int, string> EventOtherPlayerRegisteredInRoom;
    public static Action<int, string> EventLocalPlayerAssignedIndex;
    public static Action<int> EventOtherPlayerUnregisteredInRoom;

    public static Action<int, bool> NotifyClientReadyStatusChange;
    public static Action<int, bool, string> EventOtherClientReadyStatusChanged;

    public static Action ActionShowStartButton;
    public static Action ActionHideStartButton;
    public static Action ActionAllowStartButton;
    public static Action ActionDenyStartButton;
    #endregion

    #region Subscribe
    public static Action<GameObject> ActionRegisterPlayerInRoom;
    public static Action<bool, string> EventLocalReadyStatusChange; // we get player name on positive ready status change
    public static Action EventStartGameButtonPress;
    #endregion

    [SerializeField]
    private int _minimumReadyPlayerCount = 2;

    private Dictionary<int, Player> _playersServer;
    private Player _hostPlayer;

    private int _serverPlayerCount;
    private int _readyPlayerCount;

    private bool _isStartAllowed;

    private int _clientLocalPlayerIndex;

    public override void OnStartServer()
    {
        _playersServer = new Dictionary<int, Player>();
        _serverPlayerCount = 0;
        _readyPlayerCount = 0;
    }

    public override void OnStartClient()
    {
        Debug.Log("NetworkRoom start client");
        ActionRegisterPlayerInRoom += RegisterPlayerInRoom;

        EventLocalReadyStatusChange += OnLocalReadyStatusChange;
        EventStartGameButtonPress += OnStartGameButtonPress;
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
        string newName = $"Player {newIndex}";
        newPlayer.Name = newName;
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

        foreach (var keyValue in _playersServer)
        {
            int playerIndex = keyValue.Key;
            NetworkConnectionToClient connection = keyValue.Value.connectionToClient;
            string playerName = keyValue.Value.Name;
            TargetOtherPlayerAdded(connection, newIndex, newName);
            TargetOtherPlayerAdded(newPlayer.connectionToClient, playerIndex, playerName);
        }

        _playersServer.Add(newIndex, newPlayer);
        TargetLocalPlayerAdded(newPlayer.connectionToClient, newIndex, newName);
    }
    [TargetRpc]
    private void TargetOtherPlayerAdded(NetworkConnection connection, int playerIndex, string playerName)
    {
        EventOtherPlayerRegisteredInRoom(playerIndex, playerName);
    }
    [TargetRpc]
    private void TargetLocalPlayerAdded(NetworkConnection connection, int playerIndex, string playerName)
    {
        _clientLocalPlayerIndex = playerIndex;
        EventLocalPlayerAssignedIndex(playerIndex, playerName);
    }
    #endregion

    #region ReadyStatus
    private void OnLocalReadyStatusChange(bool newReadyStatus, string playerName)
    {
        CmdClientReadyStatusChanged(_clientLocalPlayerIndex, newReadyStatus, playerName);
    }

    [Command(requiresAuthority = false)]
    private void CmdClientReadyStatusChanged(int clientIndex, bool newReadyStatus, string playerName)
    {
        if (newReadyStatus)
        {
            _readyPlayerCount++;
            if (playerName != null)
            { 
                _playersServer[clientIndex].Name = playerName;
            }
        }
        else
        {
            _readyPlayerCount--;
        }

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

        RpcClientReadyStatusChanged(clientIndex, newReadyStatus, playerName);
    }
    [ClientRpc(includeOwner = false)]
    private void RpcClientReadyStatusChanged(int clientIndex, bool readyStatus, string playerName)
    {
        EventOtherClientReadyStatusChanged?.Invoke(clientIndex, readyStatus, playerName);
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
        GameController.ActionServerGameStart(_playersServer);
    }
}
