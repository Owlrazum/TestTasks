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

    public PlayerInputReceiver(CameraController cameraController)
    {
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
            Vector3 adjusted = _cameraController.AdjustInputDirection(inputDirection.normalized);
            if (Input.GetMouseButtonDown(0)
                || Input.GetKeyDown(KeyCode.Q))
            {
                _dashCommand.Direction = adjusted;
                return _dashCommand;
            }
            else
            {
                _moveCommand.Direction = adjusted;
                return _moveCommand;
            }
        }

        return null;
    }
}