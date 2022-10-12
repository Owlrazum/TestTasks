using UnityEngine;
using Mirror;
using TMPro;

[RequireComponent(typeof(Canvas))]
public class GameEndUICanvas : NetworkBehaviour
{
    [SerializeField]
    private TextMeshProUGUI _winTextMesh;

    [SerializeField]
    private string _winText;

    public override void OnStartServer()
    {
        GameController.EventServerGameEnded += OnServerGameEnd;
        GameController.EventServerMatchRestarted += OnServerMatchRestart;
    }

    public override void OnStopServer()
    {
        GameController.EventServerGameEnded -= OnServerGameEnd;
        GameController.EventServerMatchRestarted -= OnServerMatchRestart;
    }

    public override void OnStartClient()
    {
        Debug.Log("GameEndUICanvas startClient");
        gameObject.SetActive(false);
    }

    [Server]
    private void OnServerGameEnd(Player winner)
    {
        ClientRpcShowEndGameUI(winner.PlayerName);
    }

    [ClientRpc]
    private void ClientRpcShowEndGameUI(string winnerName)
    {
        Debug.Log("ShowEndGameUI");
        gameObject.SetActive(true);
        _winTextMesh.text += _winText + winnerName + "!";
    }

    [Server]
    private void OnServerMatchRestart()
    {
        ClientRpcHideEndGameUI();
    }

    [ClientRpc]
    private void ClientRpcHideEndGameUI()
    {
        gameObject.SetActive(false);
    }
}