using System;
using ForsakenGraves.Connection.Identifiers;
using Unity.Netcode;
using UnityEngine;

namespace ForsakenGraves.Connection.ConnectionStates
{
    public class StartingHostState : OnlineState
    {
        private RelayConnectionMethod _connectionMethod;

        public void Configure(RelayConnectionMethod connectionMethod) => _connectionMethod = connectionMethod;

        public override void Enter() => StartHost();

        public override void Exit() { }

        public override void OnServerStarted()
        {
            Debug.Log("ON SERVER STARTED");
        }

        public override void ApprovalCheck(NetworkManager.ConnectionApprovalRequest request, NetworkManager.ConnectionApprovalResponse response)
        {
            Debug.Log("ON APPROVAL CHECK");
        }

        private async void StartHost()
        {
            try
            {
                await _connectionMethod.SetupHostConnectionAsync();

                // NGO's StartHost launches everything
                if (!_connectionStateManager.NetworkManager.StartHost())
                {
                    StartHostFailed();
                }
            }
            catch (Exception)
            {
                StartHostFailed();
                throw;
            }
        }
        
        private void StartHostFailed()
        {
            _connectStatusPublisher.Publish(ConnectStatus.StartHostFailed);
            _connectionStateManager.ChangeState(_connectionStatesModel.OfflineState);
        }
    }
}