using System;

using UnityEngine;
using UnityEngine.Assertions;
using Mirror;

public class Player : NetworkBehaviour
{
    [SyncVar(hook = nameof(OnNameChange))]
    private string _name; // should be set only in network room
    public string PlayerName // since it is similar to object.name, it is called PlayerName, not just Name
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

    private void OnDestroy()
    {
        if (_character != null)
        {
            _character.EventServerHitOtherCharacter -= OnServerCharacterHitOtherCharacter;
        }
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
        _character = GetCharacter(playerCharacterNetId);
        _character.EventServerHitOtherCharacter += OnServerCharacterHitOtherCharacter;
        TargetRpcAssignCharacter(connectionToClient, playerCharacterNetId);
    }

    [TargetRpc]
    private void TargetRpcAssignCharacter(NetworkConnection conn, NetworkIdentity playerCharacterNetId)
    {
        _character = GetCharacter(playerCharacterNetId);
        _character.OnLocalPlayerAssign();
        EventLocalPlayerStateChanged = _character.EventStateChanged;
    }

    [Server]
    public void ServerRespawnCharacter(Vector3 position)
    {
        _character.Respawn(position);
    }

    private void OnServerCharacterHitOtherCharacter()
    {
        Debug.Log("Score increase");
        GameController.ServerPlayerIncreasedScore?.Invoke(this);
    }

    private PlayerCharacter GetCharacter(NetworkIdentity playerCharacterNetId)
    { 
        PlayerCharacter character = playerCharacterNetId.gameObject.GetComponent<PlayerCharacter>();
        Assert.IsNotNull(character, $"Client has not found PlayerCharacter using {playerCharacterNetId}");
        return character;
    }
}