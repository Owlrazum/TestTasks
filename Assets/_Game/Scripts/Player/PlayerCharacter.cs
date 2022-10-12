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

    public Action EventServerHitOtherCharacter;
    public Action<PlayerState> EventStateChanged; // PubPlayerStatesController:

    private PlayerStatesController _statesController;
    private PlayerInputReceiver _playerInputReceiever;
    private PlayerRenderer _playerRenderer;
    private CameraController _cameraController;

    [SyncVar(hook = nameof(OnInvincibilityChange))]
    private bool _isInvincibleSyncVar;
    public bool IsInvincible { get { return _isInvincibleSyncVar; } }

    private bool _isServerFrameInputReceived;
    private bool _isServerCollisionEntered;

    public override void OnStartServer()
    {
        PlayerAnimator animator = new PlayerAnimator(_animator, _playerParams);
        CharacterController controller = GetComponent<CharacterController>();
        _statesController = new PlayerStatesController(_playerParams, controller, animator);
        EventStateChanged = _statesController.EventStateChanged;

        _playerRenderer = new PlayerRenderer(_playerParams, _renderer);
    }

    public override void OnStartClient()
    {
        // clients only need to update renderer, 
        // the rest is updated on server through
        // NetworkTransform, NetworkAnimator
        _playerRenderer = new PlayerRenderer(_playerParams, _renderer);
    }

    public void OnLocalPlayerAssign()
    {
        Camera camera = CameraHolder.FuncGetCamera();
        camera.transform.position = transform.position + _playerParams.InitialCameraOffset;
        _cameraController = new CameraController(camera, _followTransform);
        _playerInputReceiever = new PlayerInputReceiver(_cameraController);
    }

    public void Respawn(Vector3 position)
    {
        transform.position = position;
        _statesController.Reset();
        _playerRenderer.Reset();
    }

    private void Update()
    {
        if (hasAuthority)
        {
            PlayerCommand command = _playerInputReceiever.GetCommand();
            CmdInputCommand(command);
        }
    }   

    [Command]
    private void CmdInputCommand(PlayerCommand command)
    {
        if (!_isServerFrameInputReceived) // As it turned out, two input commands could be received at the same frame.
        { 
            _statesController.ReactToCommand(command);
            _statesController.Update();
            _isServerFrameInputReceived = true;
        }
    }

    private void LateUpdate()
    {
        if (hasAuthority)
        {
            _cameraController.Update();
        }

        if (isServer)
        {
            _isServerFrameInputReceived = false;
            _isServerCollisionEntered = false;
        }
    }

    private void OnControllerColliderHit(ControllerColliderHit hit)
    {
        if (!isServer)
        {
            return;
        }
        
        if (_isServerCollisionEntered)
        {
            return;
        }

        if (hit.gameObject.layer != Layers.Player)
        {
            return;
        }

        _statesController.OnHit(hit, out PlayerCharacter otherPlayerCharacter);
        if (otherPlayerCharacter != null && !otherPlayerCharacter.IsInvincible)
        {
            EventServerHitOtherCharacter?.Invoke();
            otherPlayerCharacter.OnDashHit();
            _isServerCollisionEntered = true;
        }
    }

    [Server]
    public void OnDashHit()
    {
        StartCoroutine(InvincibilityTimer());
    }

    private IEnumerator InvincibilityTimer()
    {
        _isInvincibleSyncVar = true;
        yield return new WaitForSeconds(_playerParams.DashHitColorSwitchTime);
        _isInvincibleSyncVar = false;
    }

    private void OnInvincibilityChange(bool oldValue, bool newValue)
    {
        _playerRenderer.OnInvincibilityChange(newValue);
    }
}
