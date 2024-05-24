using System;
using ForsakenGraves.Connection.Data;
using ForsakenGraves.Connection.Identifiers;
using ForsakenGraves.Connection.Utilities;
using ForsakenGraves.Infrastructure.Data;
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
        
        public override void ApprovalCheck(NetworkManager.ConnectionApprovalRequest request, NetworkManager.ConnectionApprovalResponse response)
        {
            byte[] connectionData = request.Payload;
            ulong clientId = request.ClientNetworkId;
            
            // called before server starts. Just approve the client
            if (clientId == _connectionStateManager.NetworkManager.LocalClientId)
            {
                string payload = System.Text.Encoding.UTF8.GetString(connectionData);
                ConnectionPayload connectionPayload = JsonUtility.FromJson<ConnectionPayload>(payload);

                SessionManager<SessionPlayerData>.Instance.SetupConnectingPlayerSessionData(clientId, connectionPayload.PlayerId,
                                    new SessionPlayerData(clientId, connectionPayload.PlayerName, new NetworkGuid(), 0, true));

                // connection approval will create a player object for you
                response.Approved = true;
                response.CreatePlayerObject = true;
            }
        }
        
        public override void OnServerStarted()
        {
            _connectStatusPublisher.Publish(ConnectStatus.Success);
            
            HostingState hostingState = _connectionStatesModel.HostingState;
            _connectionStateManager.ChangeState(hostingState);
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