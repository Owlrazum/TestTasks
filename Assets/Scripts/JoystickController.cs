using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MoveMobile))]
public class JoystickController: MonoBehaviour
{
    private MoveMobile movement;
    private void Start()
    {
        movement = GetComponent<MoveMobile>();
        joystick.SnapX = false;
        joystick.SnapY = false;
    }
    [SerializeField]
    public FixedJoystick joystick;
    public void Update()
    {       
        if (joystick.gameObject.activeInHierarchy )//&& !joystick.isResetting)
        {
            float moveX = joystick.Horizontal;
            float moveZ = joystick.Vertical;

            Vector3 directionOfMove = Quaternion.Euler(0, 0, 0) * (new Vector3(moveX, 0, moveZ)).normalized;
            float speedInput = new Vector2(moveX, moveZ).magnitude;

            movement.UpdateMoveControl(speedInput, directionOfMove);
        }
    }

    public void SetActiveJoystick(bool isActive)
    {
        if (isActive == false)
        {
            movement.UpdateMoveControl(0, Vector3.zero);
            joystick.Reset();
        }
        joystick.SetActiveImages(isActive);
    }
}