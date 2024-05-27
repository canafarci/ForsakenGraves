using System;
using ForsakenGraves.Connection.Identifiers;
using UnityEngine;

namespace ForsakenGraves.Connection.ConnectionStates
{
    public class ClientConnectingState : OnlineState
    {
        private RelayConnectionMethod _connectionMethod;

        public void Configure(RelayConnectionMethod connectionMethod)
        {
            _connectionMethod = connectionMethod;
        }
        
        public override void Enter()
        {
            ConnectClientAsync();
        }

        public override void Exit() { }
        
        public override void OnClientConnected(ulong _)
        {
            _connectStatusPublisher.Publish(ConnectStatus.Success);
            
            ClientConnectedState clientConnectedState = _connectionStatesModel.ClientConnectedState;
            _connectionStateManager.ChangeState(clientConnectedState);
        }

        public override void OnClientDisconnect(ulong _)
        {
            // client ID is for sure ours here
            StartingClientFailed();
        }
        
        private async void ConnectClientAsync()
        {
            try
            {
                // Setup NGO with current connection method
                await _connectionMethod.SetupClientConnectionAsync();

                // NGO's StartClient launches everything
                if (!_connectionStateManager.NetworkManager.StartClient())
                {
                    throw new Exception("NetworkManager StartClient failed");
                }
            }
            catch (Exception e)
            {
                Debug.LogError("Error connecting client, see following exception");
                Debug.LogException(e);
                StartingClientFailed();
                throw;
            }
        }
        
        private void StartingClientFailed()
        {
            var disconnectReason = _connectionStateManager.NetworkManager.DisconnectReason;
            if (string.IsNullOrEmpty(disconnectReason))
            {
                _connectStatusPublisher.Publish(ConnectStatus.StartClientFailed);
            }
            else
            {
                ConnectStatus connectStatus = JsonUtility.FromJson<ConnectStatus>(disconnectReason);
                _connectStatusPublisher.Publish(connectStatus);
            }

            OfflineState offlineState =  _connectionStatesModel.OfflineState;
            _connectionStateManager.ChangeState(offlineState);
        }
    }
}