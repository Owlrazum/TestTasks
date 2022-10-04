using System;

using UnityEngine;

public class PlayerInputReceiver
{
    /// "Pascal Case. The use of SCREAMING_CAPS is discouraged. This is a large change from earlier conventions. 
    /// Most developers now realize that in using SCREAMING_CAPS they betray more implementation than is necessary."
    /// https://en.wikibooks.org/wiki/C_Sharp_Programming/Naming
    private const string HorizontalMoveAxisName = "Horizontal";
    private const string VerticalMoveAxisName = "Vertical";

    private CameraController _cameraController;

    public PlayerInputReceiver(CameraController cameraController)
    {
        _cameraController = cameraController;
    }

    public PlayerCommand GetCommand()
    {
        float h = Input.GetAxis(HorizontalMoveAxisName);
        float v = Input.GetAxis(VerticalMoveAxisName);

        if (Input.GetMouseButtonDown(0)
            || Input.GetKeyDown(KeyCode.Q))
        {
            return new PlayerDashCommand();
        }

        if (h != 0 || v != 0)
        {
            Vector3 inputDirection = new Vector3(h, 0, v);
            Vector3 adjustedDirection = _cameraController.AdjustInputDirection(inputDirection.normalized);
            return new PlayerMoveCommand(adjustedDirection);
        }

        return null;
    }
}