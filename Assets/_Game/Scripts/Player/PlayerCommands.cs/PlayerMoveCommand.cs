using UnityEngine;

public class PlayerMoveCommand : PlayerCommand
{ 
    public Vector3 Direction { get; private set; }
    public PlayerMoveCommand(Vector3 direction)
    {
        Direction = direction;
    }
    
    public bool ShouldStartAtMaxSpeed { get; set; }

    public override string ToString()
    {
        return $"MoveCommand {Direction:F2}";
    }
}