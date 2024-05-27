using ForsakenGraves.Connection.Identifiers;
using ForsakenGraves.UnityService.Lobbies;
using UnityEngine;
using VContainer;

namespace ForsakenGraves.Connection.ConnectionStates
{
    public class ClientConnectedState : OnlineState
    {
        [Inject] private LobbyServiceFacade _lobbyServiceFacade;
        public override void Enter()
        {
            if (_lobbyServiceFacade.CurrentUnityLobby != null)
            {
                _lobbyServiceFacade.BeginTracking();
            }
        }

        public override void Exit() { }
        
        public override void OnClientDisconnect(ulong _)
        {
            string disconnectReason = _connectionStateManager.NetworkManager.DisconnectReason;
            if (string.IsNullOrEmpty(disconnectReason) ||
                disconnectReason == "Disconnected due to host shutting down.")
            {
                _connectStatusPublisher.Publish(ConnectStatus.Reconnecting);
                
                //TODO change to reconnect state
                OfflineState offlineState = _connectionStatesModel.OfflineState;
                _connectionStateManager.ChangeState(offlineState);
            }
            else
            {
                ConnectStatus connectStatus = JsonUtility.FromJson<ConnectStatus>(disconnectReason);
                _connectStatusPublisher.Publish(connectStatus);

                OfflineState offlineState = _connectionStatesModel.OfflineState;
                _connectionStateManager.ChangeState(offlineState);
            }
        }
    }
}