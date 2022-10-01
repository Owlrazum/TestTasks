using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class Player : MonoBehaviour
{
    [SerializeField]
    private PlayerParamsSO _playerParams;

    [SerializeField]
    private Camera _camera;

    [SerializeField]
    private Animator _animator;

    private PlayerStatesController _statesController;
    private PlayerInputReceiver _playerInputReceiever;
    private void Awake()
    {
        CameraController cameraController = new CameraController(_camera);
        PlayerAnimator animator = new PlayerAnimator(_animator ,_playerParams);

        CharacterController controller = GetComponent<CharacterController>();
        _playerInputReceiever = new PlayerInputReceiver(cameraController);
        _statesController = new PlayerStatesController(_playerParams, controller, animator);
    }

    private void Update()
    {
        PlayerCommand command = _playerInputReceiever.GetCommand();
        _statesController.ReactToCommand(command);
        _statesController.Update();
    }
}
