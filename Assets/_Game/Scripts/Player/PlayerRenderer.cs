using UnityEngine;

public class PlayerRenderer
{
    private Material _defaultMaterial;
    private Material _invincibleMaterial;
    private SkinnedMeshRenderer _renderer;

    public PlayerRenderer(PlayerParamsSO playerParams, SkinnedMeshRenderer renderer)
    {
        _defaultMaterial = playerParams.DefaultMaterial;
        _invincibleMaterial = playerParams.InvincibleMaterial;
        _renderer = renderer;
    }

    public void OnInvincibilityChange(bool newIsInvincible)
    {
        if (newIsInvincible)
        {
            _renderer.sharedMaterial = _invincibleMaterial;
        }
        else
        {
            _renderer.sharedMaterial = _defaultMaterial;
        }
    }

    public void Reset()
    {
        _renderer.sharedMaterial = _defaultMaterial;
    }
}