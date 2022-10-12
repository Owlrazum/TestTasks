using System;

public enum PlayerCommandType:byte
{
    NoCommand,
    Move = 1,
    Dash = 2
}
public abstract class PlayerCommand
{
    public abstract PlayerCommandType GetPlayerCommandType();
}