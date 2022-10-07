using UnityEngine;
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

    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
    }

    public override void OnStartLocalPlayer()
    {
        GameDelegatesContainer.LocalPlayerScoredPoint += OnLocalPlayerScoredPoint;
        NetworkDelegatesContainer.RegisterPlayerInRoom(gameObject);
    }

    private void OnDestroy()
    {
        if (isLocalPlayer)
        {
            GameDelegatesContainer.LocalPlayerScoredPoint -= OnLocalPlayerScoredPoint;
        }
    }

    private void OnLocalPlayerScoredPoint()
    {
        _score++;
        if (_score == 3)
        {
            GameDelegatesContainer.LocalGameWon?.Invoke();
        }
    }
}