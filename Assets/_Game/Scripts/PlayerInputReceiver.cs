using System;

using UnityEngine;

public class PlayerInputReceiver
{
    /// <summary>
    /// "Pascal Case. The use of SCREAMING_CAPS is discouraged. This is a large change from earlier conventions. 
    /// Most developers now realize that in using SCREAMING_CAPS they betray more implementation than is necessary."
    /// https://en.wikibooks.org/wiki/C_Sharp_Programming/Naming
    /// </summary>
    private const string HorizontalMoveAxisName = "Horizontal";
    private const string VerticalMoveAxisName = "Vertical";

    private CameraController _cameraController;

    private PlayerMoveCommand _moveCommand;
    private PlayerDashCommand _dashCommand;

    private float _dashDistance;
    private float _moveSpeed;


    public PlayerInputReceiver(PlayerParamsSO playerParamsSO, CameraController cameraController)
    {
        _moveSpeed = playerParamsSO.MoveSpeed;

        _dashDistance = playerParamsSO.DashDistance;

        _moveCommand = new PlayerMoveCommand();
        _dashCommand = new PlayerDashCommand();

        _cameraController = cameraController;
    }

    public PlayerCommand GetCommand()
    {
        float h = Input.GetAxis(HorizontalMoveAxisName);
        float v = Input.GetAxis(VerticalMoveAxisName);
        if (h != 0 || v != 0)
        {
            Vector3 inputDirection = new Vector3(h, 0, v);
            inputDirection = _cameraController.AdjustInputDirection(inputDirection.normalized);
            if (Input.GetMouseButtonDown(0))
            {
                _dashCommand.Direction = inputDirection;
                _dashCommand.Distance = _dashDistance;
                return _dashCommand;
            }
            else
            {
                _moveCommand.Direction = inputDirection;
                _moveCommand.Speed = _moveSpeed;
                return _moveCommand;
            }
        }

        return null;
    }
}