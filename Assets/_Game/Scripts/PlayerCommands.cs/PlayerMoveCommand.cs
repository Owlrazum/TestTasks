using UnityEngine;

public class PlayerMoveCommand : PlayerCommand
{ 
    public Vector3 Direction { get; set; }
    public bool ShouldStartAtMaxSpeed { get; set; }

    public override string ToString()
    {
        return $"MoveCommand {Direction:F2}";
    }
}