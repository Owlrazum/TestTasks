using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
using Mirror;
using TMPro;

[RequireComponent(typeof(Canvas))] // should be placed on separate ui for optimizations
public class PlayerScoresUI : NetworkBehaviour
{
    private TextMeshProUGUI[] _textMeshes; // assumption: sorted by their vertical position
    private List<PlayerScore> _playerScores; // key: playerIndex

    private struct PlayerScore : IComparable<PlayerScore>, IEquatable<PlayerScore>
    {
        public int PlayerIndex;
        public string PlayerName;
        public int Amount;

        public PlayerScore(int playerIndex, string playerName, int scoreAmount)
        {
            PlayerIndex = playerIndex;
            PlayerName = playerName;
            Amount = scoreAmount;
        }

        public int CompareTo(PlayerScore other)
        {
            if (Amount != other.Amount)
            { 
                return Amount.CompareTo(other.Amount);
            }
            else
            {
                return PlayerIndex.CompareTo(other.PlayerIndex);
            }
        }

        public bool Equals(PlayerScore other)
        {
            return Amount.Equals(other.Amount);
        }
    }

    public override void OnStartServer()
    {
        GameController.EventServerGameStarted += ServerInitialize;
        GameController.ServerPlayerIncreasedScore += ServerOnPlayerIncreasedScore;
    }

    public override void OnStopServer()
    {
        GameController.EventServerGameStarted -= ServerInitialize;
        GameController.ServerPlayerIncreasedScore -= ServerOnPlayerIncreasedScore;
    }

    public override void OnStartClient()
    {
        Assert.IsTrue(transform.childCount == 1, "Revisit PlayerScoresUI class, it assumes certain hierarchy");
        Transform textMeshesParent = transform.GetChild(0);
        _textMeshes = new TextMeshProUGUI[textMeshesParent.childCount];
        for (int i = 0; i < textMeshesParent.childCount; i++)
        {
            _textMeshes[i] = textMeshesParent.GetChild(i).GetComponent<TextMeshProUGUI>();
            Assert.IsNotNull(_textMeshes[i], $"The child {i} does not contain TextMeshProUGUI!");
        }
    }

    [Server]
    private void ServerInitialize(Dictionary<int, Player> players)
    {
        _playerScores = new List<PlayerScore>(players.Count);
        foreach (var kv in players)
        {
            int playerIndex = kv.Key;
            Assert.IsTrue(playerIndex < _textMeshes.Length, "The player index is out of bounds for textMeshes array!"); // perhaps redesign of player indexing will be needed in such case.
            Player player = kv.Value;
            PlayerScore score = new PlayerScore(playerIndex, player.Name, 0);
            _playerScores.AddSorted(score);
            ClientRpcUpdateTextMesh(playerIndex, score);
        }
    }

    [ClientRpc]
    private void ClientRpcUpdateTextMesh(int place, PlayerScore score)
    {
        _textMeshes[place].text = score.PlayerName + $": {score.Amount}";
    }

    [Server]
    private void ServerOnPlayerIncreasedScore(int playerIndex)
    {
        PlayerScore dummy = new PlayerScore(playerIndex, "", -1);
        int scoreIndex = _playerScores.BinarySearch(dummy, new ByPlayerIndexComparer());
        Assert.IsTrue(scoreIndex >= 0);

        PlayerScore score = _playerScores[scoreIndex];
        score.Amount++;
        _playerScores.RemoveAt(scoreIndex);
        int newIndex = _playerScores.AddSorted(score);
        if (scoreIndex != newIndex)
        {
            for (int i = newIndex; i < _playerScores.Count; i++)
            {
                ClientRpcUpdateTextMesh(i, _playerScores[i]);
            }
        }
    }

    private class ByPlayerIndexComparer : IComparer<PlayerScore>
    {
        public int Compare(PlayerScore score, PlayerScore other)
        {
            return score.PlayerIndex.CompareTo(other.PlayerIndex);
        }
    }
}