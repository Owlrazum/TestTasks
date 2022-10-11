using UnityEngine;
using TMPro;

[RequireComponent(typeof(TextMeshProUGUI))]
public class PlayerStateUIShower : MonoBehaviour
{
    private TextMeshProUGUI _textMesh;
    
    private void Awake()
    {
        TryGetComponent(out _textMesh);
        Player.EventLocalPlayerStateChanged += OnStateChanged;
    }

    private void OnDestroy()
    { 
        Player.EventLocalPlayerStateChanged -= OnStateChanged;
    }

    private void OnStateChanged(PlayerState state)
    {
        _textMesh.text = state.ToString();
    }
}