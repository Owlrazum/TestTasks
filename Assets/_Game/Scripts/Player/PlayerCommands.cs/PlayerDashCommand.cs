using UnityEngine;

public class PlayerDashCommand : PlayerCommand
{
    public Vector3 Direction { get; set; }

    public override PlayerCommandType GetPlayerCommandType()
    {
        return PlayerCommandType.Dash;
    }

    public override string ToString()
    {
        return $"DashCommand {Direction:F2}";
    }
}