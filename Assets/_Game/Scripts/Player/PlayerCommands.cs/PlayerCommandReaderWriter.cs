using System;
using Mirror;

public static class PlayerCommandReaderWriter
{
    public static void WritePlayerCommand(this NetworkWriter writer, PlayerCommand playerCommand)
    {
        switch (playerCommand)
        { 
            case null:
                writer.WriteByte((byte)PlayerCommandType.NoCommand);
                break;
            case PlayerMoveCommand moveCommand:
                writer.WriteByte((byte)PlayerCommandType.Move);
                writer.WriteVector3(moveCommand.Direction);
                break;
            case PlayerDashCommand dashCommand:
                writer.WriteByte((byte)PlayerCommandType.Dash);
                writer.WriteVector3(dashCommand.Direction);
                break;
            default:
                throw new ArgumentException($"Unknown type of player command: {playerCommand}!");
        }
    }

    public static PlayerCommand ReadPlayerCommand(this NetworkReader reader)
    {
        byte commandType = reader.ReadByte();
        switch ((PlayerCommandType)commandType)
        { 
            case PlayerCommandType.NoCommand:
                return null;
            case PlayerCommandType.Move:
                return new PlayerMoveCommand(reader.ReadVector3());
            case PlayerCommandType.Dash:
                PlayerDashCommand dashCommand = new PlayerDashCommand();
                dashCommand.Direction = reader.ReadVector3();
                return dashCommand;
            default:
                throw new ArgumentException($"Unknown type of player command byte index: {commandType}!");
        }
    }
}