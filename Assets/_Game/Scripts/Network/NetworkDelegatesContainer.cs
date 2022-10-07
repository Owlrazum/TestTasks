using System;
using UnityEngine;

public static class NetworkDelegatesContainer
{
    public static Action StartServer;
    public static Action StartHost;
    public static Action StartClient;

    public static Action<string> UpdateNetworkAddress;

    public static Action<GameObject> RegisterPlayerInRoom;
    public static Action<int> EventOtherPlayerRegisteredInRoom;
    public static Action<int> EventLocalPlayerAssignedIndex;
    public static Action<int> EventOtherPlayerUnregisteredInRoom;
    

    public static Action<int, bool> NotifyClientReadyStatusChange;
    public static Action<int, bool> EventOtherClientReadyStatusChanged;

    public static Action EventStartGame;
}