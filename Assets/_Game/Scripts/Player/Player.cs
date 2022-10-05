using System.Collections;
using UnityEngine;
using Mirror;

[RequireComponent(typeof(CharacterController))]
public class Player : NetworkBehaviour
{
    [SerializeField]
    private PlayerParamsSO _playerParams;

    [SerializeField]
    private Animator _animator;

    [SerializeField]
    private SkinnedMeshRenderer _renderer;

    [SerializeField]
    private Transform _followTransform;

    private PlayerStatesController _statesController;
    private PlayerInputReceiver _playerInputReceiever;
    private PlayerRenderer _playerRenderer;
    private CameraController _cameraController;

    private bool _isInvincible;
    public bool IsInvincible { get { return _isInvincible; } }

    public override void OnStartLocalPlayer()
    {
        _cameraController = new CameraController(GameDelegatesContainer.GetRenderingCamera(), _followTransform);
        _playerInputReceiever = new PlayerInputReceiver(_cameraController);
    }   

    public override void OnStartClient()
    {
        PlayerAnimator animator = new PlayerAnimator(_animator, _playerParams);
        CharacterController controller = GetComponent<CharacterController>();
        _statesController = new PlayerStatesController(_playerParams, controller, animator);
        _playerRenderer = new PlayerRenderer(_playerParams, _renderer);
    }

    private void Update()
    {
        if (isLocalPlayer)
        { 
            PlayerCommand command = _playerInputReceiever.GetCommand();
            CmdInputCommand(command);
            _statesController.ReactToCommand(command);
            _statesController.Update();
        }
    }

    private void LateUpdate()
    {
        if (isLocalPlayer)
        { 
            _cameraController.Update();
        }
    }

    private void OnControllerColliderHit(ControllerColliderHit hit)
    {
        if (hit.gameObject.layer != Layers.Player)
        {
            return;
        }
        _statesController.OnHit(hit, out Player otherPlayer);
        if (otherPlayer != null)
        {
            Debug.Log("Hit other");
            otherPlayer.OnDashHit();
        }
    }

    public void OnDashHit()
    {
        if (_isInvincible)
        {
            Debug.Log("Invicible");
            return;
        }

        StartCoroutine(InvincibilityTimer());
    }

    private IEnumerator InvincibilityTimer()
    {
        SetInvincibility(true);
        yield return new WaitForSeconds(_playerParams.DashHitColorSwitchTime);
        SetInvincibility(false);
    }

    private void SetInvincibility(bool isInvincible)
    {
        Debug.Log($"I am {isInvincible}");
        _isInvincible = isInvincible;
        _playerRenderer.OnInvincibilityChange(isInvincible);
    }

    [Command]
    private void CmdInputCommand(PlayerCommand command)
    {
        RpcInputCommand(command);
    }

    [ClientRpc(includeOwner = false)]
    private void RpcInputCommand(PlayerCommand command)
    { 
        _statesController.ReactToCommand(command);
        _statesController.Update();
    }
}
