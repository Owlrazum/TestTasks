using System;

public static class UIDelegatesContainer
{
    public static Func<RoomUI> GetRoomUI;
    public static Action<bool> EventLocalReadyStatusChange;
}