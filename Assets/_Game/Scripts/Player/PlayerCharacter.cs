using System;
using System.Collections;
using UnityEngine;
using Mirror;

[RequireComponent(typeof(CharacterController))]
public class PlayerCharacter : NetworkBehaviour
{
    [SerializeField]
    private PlayerParamsSO _playerParams;

    [SerializeField]
    private Animator _animator;

    [SerializeField]
    private SkinnedMeshRenderer _renderer;

    [SerializeField]
    private Transform _followTransform;

    public Action EventHitOtherCharacter;
    public Action EventStateChanged; // PubPlayerStatesController:

    private PlayerStatesController _statesController;
    private PlayerInputReceiver _playerInputReceiever;
    private PlayerRenderer _playerRenderer;
    private CameraController _cameraController;

    private bool _isInvincible;
    public bool IsInvincible { get { return _isInvincible; } }

    private float _syncTimer;

    public override void OnStartClient()
    {
        PlayerAnimator animator = new PlayerAnimator(_animator, _playerParams);
        CharacterController controller = GetComponent<CharacterController>();
        _statesController = new PlayerStatesController(_playerParams, controller, animator);
        _playerRenderer = new PlayerRenderer(_playerParams, _renderer);
    }

    public void OnPlayerAssign()
    {
        GameController.EventLocalPlayerCharacterSpawned?.Invoke(this);

        Camera camera = CameraHolder.FuncGetCamera();
        camera.transform.position = transform.position + _playerParams.InitialCameraOffset;
        _cameraController = new CameraController(camera, _followTransform);
        _playerInputReceiever = new PlayerInputReceiver(_cameraController);
    }

    private void Update()
    {
        if (hasAuthority)
        {
            PlayerCommand command = _playerInputReceiever.GetCommand();
            CmdInputCommand(command);
            _statesController.ReactToCommand(command);
            _statesController.Update();

            _syncTimer += Time.deltaTime;
            if (_syncTimer > _playerParams.SyncTime)
            {
                _syncTimer = 0;
                CmdSyncPosition(transform.position);
            }
        }
    }   

    private void LateUpdate()
    {
        if (hasAuthority)
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
        _statesController.OnHit(hit, out PlayerCharacter otherPlayer);
        if (otherPlayer != null && !otherPlayer.IsInvincible)
        {
            EventHitOtherCharacter?.Invoke();
            otherPlayer.OnDashHit();
        }
    }

    public void OnDashHit()
    {
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

    [Command]
    private void CmdSyncPosition(Vector3 position)
    {
        ClientRpcSyncPosition(position);
    }

    [ClientRpc]
    private void ClientRpcSyncPosition(Vector3 position)
    {
        transform.position = position;
    }

    [ClientRpc(includeOwner = false)]
    private void RpcInputCommand(PlayerCommand command)
    {
        _statesController.ReactToCommand(command);
        _statesController.Update();
    }
}
