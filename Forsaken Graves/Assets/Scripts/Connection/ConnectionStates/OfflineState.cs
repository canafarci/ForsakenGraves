using ForsakenGraves.Identifiers;
using ForsakenGraves.Infrastructure.SceneManagement.Signals;
using ForsakenGraves.UnityService.Lobbies;
using MessagePipe;
using Unity.Netcode;
using VContainer;

namespace ForsakenGraves.Connection.ConnectionStates
{
    public class OfflineState : ConnectionState
    {
        [Inject] private ConnectionStateManager _connectionStateManager;
        [Inject] private IPublisher<LoadSceneSignal> _sceneLoadPublisher;
        [Inject] private LobbyServiceFacade _lobbyServiceFacade;
        [Inject] private NetworkManager _networkManager;
        
        public override void Enter()
        {
            _lobbyServiceFacade.EndTracking();
            _networkManager.Shutdown();
            
            _sceneLoadPublisher.Publish(new LoadSceneSignal(SceneIdentifier.MainMenu, false));
        }

        public override void Exit() { }
    }
}