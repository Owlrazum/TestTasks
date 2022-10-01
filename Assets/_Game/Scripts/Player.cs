using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class Player : MonoBehaviour
{
    [SerializeField]
    private PlayerParamsSO _playerParams;

    [SerializeField]
    private Camera _camera;

    private PlayerStatesController _statesController;
    private PlayerInputReceiver _playerInputReceiever;
    private void Awake()
    {
        CameraController cameraController = new CameraController(_camera);

        CharacterController controller = GetComponent<CharacterController>();
        _playerInputReceiever = new PlayerInputReceiver(_playerParams, cameraController);
        _statesController = new PlayerStatesController(controller);
    }

    private void Update()
    {
        PlayerCommand command = _playerInputReceiever.GetCommand();
        _statesController.ReactToCommand(command);
        _statesController.Update();
    }
}
