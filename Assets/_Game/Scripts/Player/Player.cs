using System;

using UnityEngine;
using UnityEngine.Assertions;
using Mirror;

public class Player : NetworkBehaviour
{
    [SyncVar]
    private int _index; // should be set only in network room
    public int Index
    {
        get { return _index; }
        set { _index = value; }
    }

    [SyncVar]
    private string _name; // should be set only in network room
    public string Name
    {
        get { return _name; }
        set { _name = value; }
    }

    public bool IsReady { get; set; }

    private PlayerCharacter _character;
    public static Action<PlayerState> EventLocalPlayerStateChanged; // hook for PlayerStateUIShower

    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
    }

    public override void OnStartLocalPlayer()
    {
        NetworkRoom.ActionRegisterPlayerInRoom(gameObject);
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
        Debug.Log("OnPlayerAssign target");
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
        GameController.ServerPlayerIncreasedScore?.Invoke(_index);
    }
}