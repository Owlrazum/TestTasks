using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

using Mirror;

public class NetworkRoom : NetworkBehaviour
{
    private Dictionary<int, Player> _players;
    
    private int _serverPlayerCount;

    private int _clientLocalPlayerIndex;

    public override void OnStartServer()
    {
        _players = new Dictionary<int, Player>();
        _serverPlayerCount = 0;
    }

    public override void OnStartClient()
    {
        NetworkDelegatesContainer.RegisterPlayerInRoom += RegisterPlayerInRoom;

        UIDelegatesContainer.EventLocalReadyStatusChange += OnLocalReadyStatusChange;
    }

    public override void OnStopClient()
    {
        NetworkDelegatesContainer.RegisterPlayerInRoom -= RegisterPlayerInRoom;

        UIDelegatesContainer.EventLocalReadyStatusChange -= OnLocalReadyStatusChange;
    }

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
        NetworkDelegatesContainer.EventOtherPlayerRegisteredInRoom(playerIndex);
    }

    [TargetRpc]
    private void TargetLocalPlayerAdded(NetworkConnection connection, int playerIndex)
    {
        _clientLocalPlayerIndex = playerIndex;
        NetworkDelegatesContainer.EventLocalPlayerAssignedIndex(playerIndex);
    }

    private void OnLocalReadyStatusChange(bool newReadyStatus)
    {
        CmdClientReadyStatusChanged(_clientLocalPlayerIndex, newReadyStatus);
    }

    [Command(requiresAuthority = false)]
    private void CmdClientReadyStatusChanged(int clientIndex, bool newReadyStatus)
    {
        RpcClientReadyStatusChanged(clientIndex, newReadyStatus);
    }

    [ClientRpc(includeOwner = false)]
    private void RpcClientReadyStatusChanged(int clientIndex, bool readyStatus)
    {
        NetworkDelegatesContainer.EventOtherClientReadyStatusChanged?.Invoke(clientIndex, readyStatus);
    }
}
