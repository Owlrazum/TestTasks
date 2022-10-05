using System;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.SceneManagement;
using Mirror;

namespace Game
{
    public class NetworkController : NetworkRoomManager
    {
        private int _enteredRoomPlayersCount;
        public override void Awake()
        {
            base.Awake();
            
            NetworkDelegatesContainer.StartServer += StartServer;
            NetworkDelegatesContainer.StartHost   += StartHost;
            NetworkDelegatesContainer.StartClient += StartClient;

            NetworkDelegatesContainer.UpdateNetworkAddress += UpdateNetworkAddress;

            Assert.IsTrue(NetworkDelegatesContainer.StartServer.GetInvocationList().Length == 1);
        }

        public override void OnDestroy()
        {
            NetworkDelegatesContainer.StartServer -= StartServer;
            NetworkDelegatesContainer.StartHost   -= StartHost;
            NetworkDelegatesContainer.StartClient -= StartClient;

            NetworkDelegatesContainer.UpdateNetworkAddress -= UpdateNetworkAddress;

            Assert.IsTrue(NetworkDelegatesContainer.StartServer == null);
        }

        private void UpdateNetworkAddress(string address)
        {
            networkAddress = address;
        }

        public override GameObject OnRoomServerCreateRoomPlayer(NetworkConnectionToClient conn)
        {
            return base.OnRoomServerCreateRoomPlayer(conn);
        }

        public override void OnGUI()
        {
        }
    }
}