using ForsakenGraves.Identifiers;
using ForsakenGraves.Infrastructure;
using ForsakenGraves.Infrastructure.SceneManagement.Messages;
using ForsakenGraves.UnityService.Lobbies;
using MessagePipe;
using Unity.Netcode;
using VContainer;

namespace ForsakenGraves.Connection.ConnectionStates
{
    public class OfflineState : ConnectionState
    {
        [Inject] private IPublisher<LoadSceneMessage> _sceneLoadPublisher;
        [Inject] private LobbyServiceFacade _lobbyServiceFacade;
        [Inject] private NetworkManager _networkManager;
        [Inject] private LocalLobby _localLobby;
        [Inject] private ProfileManager _profileManager;
        
        public override void Enter()
        {
            _lobbyServiceFacade.EndTracking();
            _networkManager.Shutdown();
            
            _sceneLoadPublisher.Publish(new LoadSceneMessage(SceneIdentifier.MainMenu, false));
        }

        public override void Exit() { }

        public override void StartHostLobby(string playerName)
        {
            RelayConnectionMethod connectionMethod = new RelayConnectionMethod(_lobbyServiceFacade,
                                                                               _localLobby,
                                                                               _connectionStateManager,
                                                                               _profileManager,
                                                                               playerName);
            
            StartingHostState startingHostState = _connectionStatesModel.StartingHostState;
            
            startingHostState.Configure(connectionMethod);
            _connectionStateManager.ChangeState(startingHostState);
            
        }

        public override void StartClientLobby(string playerName)
        {
            RelayConnectionMethod connectionMethod = new RelayConnectionMethod(_lobbyServiceFacade,
                                                                               _localLobby,
                                                                               _connectionStateManager,
                                                                               _profileManager,
                                                                               playerName);
            
            ClientConnectingState clientConnectingState = _connectionStatesModel.ClientConnectingState;
            
            clientConnectingState.Configure(connectionMethod);
            _connectionStateManager.ChangeState(clientConnectingState);
        }
    }
}