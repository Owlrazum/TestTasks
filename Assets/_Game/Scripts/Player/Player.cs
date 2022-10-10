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

    private PlayerCharacter _character;

    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
        // NetworkRoom.NetworkRoomInitialized += OnNetworkRoomReady;
    }

    private void OnDestroy()
    { 
        // NetworkRoom.NetworkRoomInitialized -= OnNetworkRoomReady;
    }

    private void OnNetworkRoomReady()
    {
        if (isLocalPlayer)
        { 
            NetworkRoom.ActionRegisterPlayerInRoom(gameObject);
        }
    }

    public void ServerAssignCharacter(NetworkIdentity playerCharacterNetId)
    {
        ClientRpcAssignCharacter(playerCharacterNetId);
    }

    [ClientRpc]
    private void ClientRpcAssignCharacter(NetworkIdentity playerCharacterNetId)
    { 
        _character = playerCharacterNetId.gameObject.GetComponent<PlayerCharacter>();
        Assert.IsNotNull(_character, $"Client has not found PlayerCharacter using {playerCharacterNetId}");
        _character.EventHitOtherCharacter += OnCharacterHitOtherCharacter;
    }

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