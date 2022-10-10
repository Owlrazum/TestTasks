using UnityEngine;
using UnityEngine.Assertions;
using Mirror;

public class Player : NetworkBehaviour
{
    [SyncVar]
    private int _score;

    public int Score
    {
        get { return _score; }
        set { _score = value; }
    }

    [SyncVar]
    private int _index;
    public int Index
    {
        get { return _index; }
        set { _index = value; }
    }

    public bool IsReady { get; set; }

    private PlayerCharacter _character;

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
        _character.OnPlayerAssign();
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
        _score++;
    }
}