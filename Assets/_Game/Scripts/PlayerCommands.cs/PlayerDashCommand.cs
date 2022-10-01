using UnityEngine;

public class PlayerDashCommand : PlayerCommand
{
    public Vector3 Direction { get; set; }
    public float Distance { get; set; }

    public override string ToString()
    {
        return $"DashCommand {Direction:F2} {Distance:F2}";
    }
}