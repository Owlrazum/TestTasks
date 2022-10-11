using System;

using UnityEngine;
using UnityEngine.Assertions;
using Mirror;

public class Player : NetworkBehaviour
{
    [SyncVar(hook = nameof(OnNameChange))]
    private string _name; // should be set only in network room
    public string Name
    {
        get { return _name; }
        set { _name = value; }
    }

    [SyncVar(hook = nameof(OnReadyStatusChange))]
    private bool _isReady; // should be set only in network room
    public bool IsReady
    {
        get { return _isReady; }
        set { _isReady = value; }
    }

    public int Index { get; set; } // syncronised manually due to mess with syncVars and Rpcs used together.
    public bool IsLocalPlayer { get { return isLocalPlayer; } }

    private PlayerCharacter _character;
    public static Action<PlayerState> EventLocalPlayerStateChanged; // hook for PlayerStateUIShower

    public Action<Player, int, int> EventIndexChanged;
    public Action<string> EventNameChanged;
    public Action<bool> EventReadyStatusChanged;

    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
    }

    public override void OnStartLocalPlayer()
    {
        NetworkRoom.ActionRegisterPlayerInRoom(netIdentity);
    }

    private void OnNameChange(string oldName, string newName)
    { 
        EventNameChanged?.Invoke(newName);
    }

    private void OnReadyStatusChange(bool oldReadyStatus, bool newReadyStatus)
    {
        EventReadyStatusChanged?.Invoke(newReadyStatus);
    }

    [Server]
    public void ServerAssignCharacter(NetworkIdentity playerCharacterNetId)
    {
        playerCharacterNetId.AssignClientAuthority(connectionToClient);
        TargetRpcAssignCharacter(connectionToClient, playerCharacterNetId);
    }

    [TargetRpc]
    private void TargetRpcAssignCharacter(NetworkConnection conn, NetworkIdentity playerCharacterNetId)
    {
        _character = playerCharacterNetId.gameObject.GetComponent<PlayerCharacter>();
        Assert.IsNotNull(_character, $"Client has not found PlayerCharacter using {playerCharacterNetId}");
        _character.EventHitOtherCharacter += OnCharacterHitOtherCharacter;
        _character.OnLocalPlayerAssign();
        EventLocalPlayerStateChanged = _character.EventStateChanged;
    }

    [Server]
    public void ServerDisposeCharacter()
    {
        ClientRpcDisposeCharacter();
    }

    [ClientRpc]
    private void ClientRpcDisposeCharacter()
    {
        _character.EventHitOtherCharacter -= OnCharacterHitOtherCharacter;
        _character = null;
    }

    private void OnCharacterHitOtherCharacter()
    {
        CmdIncreaseScore();
    }

    [Command]
    private void CmdIncreaseScore()
    {
        GameController.ServerPlayerIncreasedScore?.Invoke(Index);
    }
}