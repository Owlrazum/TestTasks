using System;

public static class NetworkDelegatesContainer
{
    public static Action StartServer;
    public static Action StartHost;
    public static Action StartClient;

    public static Action<string> UpdateNetworkAddress;

    public static Action<bool> ClientReadyStatusChanged;
}