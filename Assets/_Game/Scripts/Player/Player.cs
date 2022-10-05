using UnityEngine;
using Cinemachine;
using Mirror;

[RequireComponent(typeof(CharacterController))]
public class Player : NetworkBehaviour
{
    [SerializeField]
    private PlayerParamsSO _playerParams;

    [SerializeField]
    private Animator _animator;

    [SerializeField]
    private CinemachineVirtualCamera _virtualCameraPrefab;

    [SerializeField]
    private Transform _followTransform;

    private PlayerStatesController _statesController;
    private PlayerInputReceiver _playerInputReceiever;
    private CameraController _cameraController;

    public override void OnStartLocalPlayer()
    {
        // CreateVirtualCamera();
        _cameraController = new CameraController(GameDelegatesContainer.GetRenderingCamera(), _followTransform);
        _playerInputReceiever = new PlayerInputReceiver(_cameraController);
    }   

    public override void OnStartClient()
    {
        PlayerAnimator animator = new PlayerAnimator(_animator, _playerParams);
        CharacterController controller = GetComponent<CharacterController>();
        _statesController = new PlayerStatesController(_playerParams, controller, animator);
    }

    private void CreateVirtualCamera()
    {
        var vCamGb = Instantiate(_virtualCameraPrefab);
        var vcam = vCamGb.GetComponent<CinemachineVirtualCamera>();
        vcam.Follow = transform;
        vcam.LookAt = transform;
    }

    private void Update()
    {
        if (isLocalPlayer)
        { 
            PlayerCommand command = _playerInputReceiever.GetCommand();
            CmdInputCommand(command);
        }
    }

    private void LateUpdate()
    {
        if (isLocalPlayer)
        { 
            _cameraController.Update();
        }
    }

    [Command]
    private void CmdInputCommand(PlayerCommand command)
    {
        RpcInputCommand(command);
    }

    [ClientRpc]
    private void RpcInputCommand(PlayerCommand command)
    { 
        _statesController.ReactToCommand(command);
        _statesController.Update();
    }
}
