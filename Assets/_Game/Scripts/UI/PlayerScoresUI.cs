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

    private int MaxTextMeshCount;
    private ByPlayerIndexComparer _playerIndexComparer;
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
                if (Amount > other.Amount)
                {
                    return -1;
                }
                else
                {
                    return 1;
                }
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

        public override string ToString()
        {
            return $"Index: {PlayerIndex} {PlayerName} {Amount}";
        }
    }

    private void Awake()
    {
        MaxTextMeshCount = transform.GetChild(0).childCount;
        _playerIndexComparer = new ByPlayerIndexComparer();
    }

    public override void OnStartServer()
    {
        GameController.EventServerGameStarted += ServerInitialize;
        GameController.ServerPlayerIncreasedScore += ServerOnPlayerIncreasedScore;
        GameController.EventServerMatchRestarted += OnServerMatchRestarted;
    }

    public override void OnStopServer()
    {
        GameController.EventServerGameStarted -= ServerInitialize;
        GameController.ServerPlayerIncreasedScore -= ServerOnPlayerIncreasedScore;
        GameController.EventServerMatchRestarted -= OnServerMatchRestarted;
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
            Assert.IsTrue(playerIndex < MaxTextMeshCount, "The player index is out of bounds for textMeshes array!"); // perhaps redesign of player indexing will be needed in such case.
            Player player = kv.Value;
            PlayerScore score = new PlayerScore(playerIndex, player.PlayerName, 0);
            _playerScores.AddSorted(score, _playerIndexComparer);
            ClientRpcUpdateTextMesh(playerIndex, score);
        }

        if (players.Count < 4)
        {
            for (int i = players.Count; i < MaxTextMeshCount; i++)
            {
                ClientRpcRemoveTextMesh(i);
            }
        }
    }

    [ClientRpc]
    private void ClientRpcUpdateTextMesh(int place, PlayerScore score)
    {
        _textMeshes[place].text = score.PlayerName + $": {score.Amount}";
    }

    [ClientRpc]
    private void ClientRpcRemoveTextMesh(int place)
    {
        _textMeshes[place].gameObject.SetActive(false);
    }

    [Server]
    private void ServerOnPlayerIncreasedScore(Player player)
    {
        PlayerScore dummy = new PlayerScore(player.Index, "", -1);
        int scoreIndex = _playerScores.BinarySearch(dummy, _playerIndexComparer);
        Assert.IsTrue(scoreIndex >= 0);

        PlayerScore score = _playerScores[scoreIndex];
        score.Amount += 1;
        _playerScores[scoreIndex] = score;

        _playerScores.Sort();
        for (int i = 0; i < _playerScores.Count; i++)
        {
            ClientRpcUpdateTextMesh(i, _playerScores[i]);
        }
    }

    private class ByPlayerIndexComparer : IComparer<PlayerScore>
    {
        public int Compare(PlayerScore score, PlayerScore other)
        {
            return score.PlayerIndex.CompareTo(other.PlayerIndex);
        }
    }

    [Server]
    private void OnServerMatchRestarted()
    {
        for (int i = 0; i < _playerScores.Count; i++)
        {
            PlayerScore playerScore = _playerScores[i];
            playerScore.Amount = 0;
            _playerScores[i] = playerScore;

            ClientRpcUpdateTextMesh(i, _playerScores[i]);
        }
    }
}