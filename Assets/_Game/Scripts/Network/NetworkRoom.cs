using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

using Mirror;

public class NetworkRoom : NetworkBehaviour
{
    #region Publish
    public static Action<Player> EventPlayerRegistered;
    // public static Action<Player> EventPlayerUnregistered; // due to shortage in time, decision was made to not add this feature.

    public static Action ActionShowStartButton;
    public static Action ActionHideStartButton;
    public static Action ActionAllowStartButton;
    public static Action ActionDenyStartButton;
    #endregion

    #region Subscribe
    public static Action<NetworkIdentity> ActionRegisterPlayerInRoom;
    public static Action ActionChangeLocalPlayerReadyStatus; // we get player name on positive ready status change
    public static Action<string> ActionChangeLocalPlayerName;
    public static Action EventStartGameButtonPress;
    #endregion

    [SerializeField]
    private int _minimumReadyPlayerCount = 2;

    private Dictionary<int, Player> _playersServer;
    private NetworkConnection _serverHostConnection;
    private Player _localPlayer;

    private int _serverPlayerCount;
    private int _readyPlayerCount;

    private bool _isStartAllowed;

    public override void OnStartServer()
    {
        _playersServer = new Dictionary<int, Player>();
        _serverPlayerCount = 0;
        _readyPlayerCount = 0;
    }

    public override void OnStartClient()
    {
        ActionRegisterPlayerInRoom += RegisterPlayerInRoom;

        ActionChangeLocalPlayerReadyStatus += ChangeLocalPlayerReadyStatus;
        ActionChangeLocalPlayerName += ChangeLocalPlayerName;
        EventStartGameButtonPress += OnStartGameButtonPress;
    }

    public override void OnStopClient()
    {
        ActionRegisterPlayerInRoom -= RegisterPlayerInRoom;

        ActionChangeLocalPlayerReadyStatus -= ChangeLocalPlayerReadyStatus;
        ActionChangeLocalPlayerName -= ChangeLocalPlayerName;
        
        EventStartGameButtonPress -= OnStartGameButtonPress;
    }

    #region Registration
    private void RegisterPlayerInRoom(NetworkIdentity playerNetId)
    {
        CmdRegisterPlayer(playerNetId);
    }
    [Command(requiresAuthority = false)]
    private void CmdRegisterPlayer(NetworkIdentity newPlayerNetId)
    {
        Player newPlayer = GetPlayer(newPlayerNetId);
        ServerInitializePlayer(newPlayer);
        _serverPlayerCount++;

        ServerShowOrHideStartButton(newPlayer);
        ServerNotifyOnPlayerRegister(newPlayer);
    }

    [Server]
    private void ServerInitializePlayer(Player newPlayer)
    {
        int newIndex = _serverPlayerCount;
        newPlayer.Index = newIndex;
        string newName = $"Player {newIndex}";
        newPlayer.PlayerName = newName;
    }

    [Server]
    private void ServerShowOrHideStartButton(Player newPlayer)
    { 
        if (_serverPlayerCount == 1)
        {
            _serverHostConnection = newPlayer.connectionToClient;
            TargetShowStartButton(_serverHostConnection);
        }
        else
        {
            TargetHideStartButton(newPlayer.connectionToClient);
        }
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

    [Server]
    private void ServerNotifyOnPlayerRegister(Player newPlayer)
    { 
        NetworkConnectionToClient newConnection = newPlayer.connectionToClient;
        NetworkIdentity newPlayerNetId = newPlayer.netIdentity;
        foreach (var keyValue in _playersServer)
        {
            Player player = keyValue.Value;
            NetworkConnectionToClient connection = player.connectionToClient;
            NetworkIdentity playerNetId = player.netIdentity;
            TargetOtherPlayerAdded(newConnection, playerNetId, player.Index);
            TargetOtherPlayerAdded(connection, newPlayerNetId, newPlayer.Index);
            if (player.IsReady)
            {
                TargetUpdateUIIfOtherReady(newConnection, playerNetId);
            }
        }

        // TODO: understand why TargetLocalPlayerAdded should be called before TargetOtherPlayerAdded
        TargetLocalPlayerAdded(newConnection, newPlayerNetId, newPlayer.Index);

        _playersServer.Add(newPlayer.Index, newPlayer);
    }
    [TargetRpc]
    private void TargetLocalPlayerAdded(NetworkConnection connection, NetworkIdentity playerNetId, int index)
    {
        _localPlayer = GetPlayer(playerNetId);
        _localPlayer.Index = index;
        EventPlayerRegistered?.Invoke(_localPlayer);
    }
    [TargetRpc]
    private void TargetOtherPlayerAdded(NetworkConnection connection, NetworkIdentity playerNetId, int index)
    {
        Player player = GetPlayer(playerNetId);
        player.Index = index;
        EventPlayerRegistered?.Invoke(player);
    }
    [TargetRpc]
    private void TargetUpdateUIIfOtherReady(NetworkConnection connection, NetworkIdentity otherPlayerNetId)
    {
        Player player = GetPlayer(otherPlayerNetId);
        player.EventReadyStatusChanged?.Invoke(true);
    }
    #endregion

    #region ReadyStatus
    private void ChangeLocalPlayerReadyStatus()
    {
        CmdChangeReadyStatus(_localPlayer.netIdentity);
    }

    [Command(requiresAuthority = false)]
    private void CmdChangeReadyStatus(NetworkIdentity playerNetId)
    {
        Player player = GetPlayer(playerNetId);
        if (player.IsReady)
        {
            player.IsReady = false;
            _readyPlayerCount--;
        }
        else
        {
            player.IsReady = true;
            _readyPlayerCount++;
        }

        if (_readyPlayerCount >= _minimumReadyPlayerCount && !_isStartAllowed)
        {
            _isStartAllowed = true;
            TargetAllowStartButton(_serverHostConnection);
        }
        else if (_readyPlayerCount < _minimumReadyPlayerCount && _isStartAllowed)
        {
            _isStartAllowed = false;
            TargetDenyStartButton(_serverHostConnection);
        }
    }
    [TargetRpc]
    private void TargetAllowStartButton(NetworkConnection connection)
    {
        ActionAllowStartButton?.Invoke();
    }
    [TargetRpc]
    private void TargetDenyStartButton(NetworkConnection connection)
    {
        ActionDenyStartButton?.Invoke();
    }
    #endregion


    private void ChangeLocalPlayerName(string newName)
    {
        CmdChangeName(_localPlayer.netIdentity, newName);
    }
    [Command(requiresAuthority = false)]
    private void CmdChangeName(NetworkIdentity playerNetId, string newName)
    {
        Player player = GetPlayer(playerNetId);
        player.PlayerName = newName;
    }

    private void OnStartGameButtonPress()
    {
        CmdStartGame();
    }
    [Command(requiresAuthority = false)]
    private void CmdStartGame()
    {
        GameController.ActionServerGameStart(_playersServer);
    }

    private Player GetPlayer(NetworkIdentity netId)
    {
        Player player = netId.GetComponent<Player>();
        Assert.IsNotNull(player, $"The player is not present on {netId}");
        return player;
    }
}
