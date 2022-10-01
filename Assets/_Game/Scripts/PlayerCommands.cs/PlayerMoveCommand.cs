using UnityEngine;

public class PlayerMoveCommand : PlayerCommand
{ 
    public Vector3 Direction { get; set; }
    public float Speed { get; set; }
    
    public override string ToString()
    {
        return $"MoveCommand {Speed:F2} {Direction:F2}";
    }
}