using ForsakenGraves.Connection.Data;
using ForsakenGraves.Connection.Identifiers;
using ForsakenGraves.Connection.Utilities;
using ForsakenGraves.Identifiers;
using ForsakenGraves.Infrastructure.Data;
using ForsakenGraves.Infrastructure.SceneManagement.Signals;
using ForsakenGraves.UnityService.Lobbies;
using MessagePipe;
using Unity.Netcode;
using UnityEngine;
using VContainer;

namespace ForsakenGraves.Connection.ConnectionStates
{
    public class HostingState : ConnectionState
    {
        [Inject] private IPublisher<LoadSceneSignal> _sceneLoadPublisher;
        [Inject] IPublisher<ConnectionEventMessage> _connectionEventPublisher;
        [Inject] private LobbyServiceFacade _lobbyServiceFacade;
        [Inject] private ConnectionStatesModel _connectionStatesModel;

        private const int MAX_PAYLOAD_SIZE = 1024;
        
        public override void Enter()
        {
            //load char select scene when hosting start via networkmanager
            _sceneLoadPublisher.Publish(new LoadSceneSignal(SceneIdentifier.CharSelectScene, useNetworkManager: true));
            
            if (_lobbyServiceFacade.CurrentUnityLobby == null) return;
            _lobbyServiceFacade.BeginTracking();
        }

        public override void Exit()
        {
            SessionManager<SessionPlayerData>.Instance.OnServerEnded();
        }

        public override void OnClientConnectedCallback(ulong clientId)
        {
            SessionPlayerData? playerData = SessionManager<SessionPlayerData>.Instance.GetPlayerData(clientId);

            if (playerData != null)
            {
                _connectionEventPublisher.Publish(new ConnectionEventMessage()
                                                  {
                                                      ConnectStatus = ConnectStatus.Success,
                                                      PlayerName = playerData.Value.PlayerName
                                                  });
            }
            else
            {
                // This should not happen since player data is assigned during connection approval
                Debug.LogError($"No player data associated with client {clientId}");
                var reason = JsonUtility.ToJson(ConnectStatus.GenericDisconnect);
                _connectionStateManager.NetworkManager.DisconnectClient(clientId, reason);
            }
        }

        public override void OnClientDisconnectCallback(ulong clientId)
        {
            if (clientId == _connectionStateManager.NetworkManager.LocalClientId) return;

            string playerID = SessionManager<SessionPlayerData>.Instance.GetPlayerID(clientId);
            if (playerID == null) return;

            SessionPlayerData? sessionPlayerData = SessionManager<SessionPlayerData>.Instance.GetPlayerData(playerID);
            if (sessionPlayerData.HasValue)
            {
                _connectionEventPublisher.Publish(new ConnectionEventMessage()
                                                  {
                                                      ConnectStatus = ConnectStatus.GenericDisconnect,
                                                      PlayerName = sessionPlayerData.Value.PlayerName
                                                  });
            }
            
            SessionManager<SessionPlayerData>.Instance.DisconnectClient(clientId);
        }
        
        //host ended lobby
        public override void OnUserRequestedShutdown()
        {
            string reason = JsonUtility.ToJson(ConnectStatus.HostEndedSession);
            for (int i = _connectionStateManager.NetworkManager.ConnectedClientsIds.Count - 1; i >= 0; i--)
            {
                ulong id = _connectionStateManager.NetworkManager.ConnectedClientsIds[i];
                if (id != _connectionStateManager.NetworkManager.LocalClientId)
                {
                    _connectionStateManager.NetworkManager.DisconnectClient(id, reason);
                }
            }
            
            //switch to offline state
            OfflineState offlineState = _connectionStatesModel.OfflineState;
            _connectionStateManager.ChangeState(offlineState);
        }
        
        public override void OnServerStopped()
        {
            _connectStatusPublisher.Publish(ConnectStatus.GenericDisconnect);
            //switch to offline state
            OfflineState offlineState = _connectionStatesModel.OfflineState;
            _connectionStateManager.ChangeState(offlineState);        }

        public override void ApprovalCheck(NetworkManager.ConnectionApprovalRequest request, NetworkManager.ConnectionApprovalResponse response)
        {
            byte[] connectionData = request.Payload;
            if (connectionData.Length > MAX_PAYLOAD_SIZE)
            {
                //request data shouldnt be this big, reject it early
                response.Approved = false;
            }

            ulong clientId = request.ClientNetworkId;
            string payload = System.Text.Encoding.UTF8.GetString(connectionData);
            ConnectionPayload connectionPayload = JsonUtility.FromJson<ConnectionPayload>(payload);
            ConnectStatus gameReturnStatus = GetConnectStatus(connectionPayload);

            if (gameReturnStatus == ConnectStatus.Success)
            {
                SessionManager<SessionPlayerData>.Instance.SetupConnectingPlayerSessionData(clientId,
                    connectionPayload.PlayerId, new SessionPlayerData(clientId,
                                                                      connectionPayload.PlayerName,
                                                                      new NetworkGuid(),
                                                                      0,
                                                                      true)); 
                //approval creates a player object
                response.Approved = true;
                response.CreatePlayerObject = true;
                response.Position = Vector3.zero;
                response.Rotation = Quaternion.identity;
                return;
            }

            response.Approved = false;
            response.Reason = JsonUtility.ToJson(gameReturnStatus);
            
            if (_lobbyServiceFacade.CurrentUnityLobby != null)
            {
                _lobbyServiceFacade.RemovePlayerFromLobbyAsync(connectionPayload.PlayerId);
            }
        }

        private ConnectStatus GetConnectStatus(ConnectionPayload connectionPayload)
        {
            if (_connectionStateManager.NetworkManager.ConnectedClientsIds.Count >=
                _connectionStateManager.MaxConnectedPlayers)
            {
                return ConnectStatus.ServerFull;
            }

            return SessionManager<SessionPlayerData>.Instance.IsDuplicateConnection(connectionPayload.PlayerId)
                       ? ConnectStatus.LoggedInAgain
                       : ConnectStatus.Success;
        }
    }
}