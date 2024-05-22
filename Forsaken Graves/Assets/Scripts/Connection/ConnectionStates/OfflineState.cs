using ForsakenGraves.Infrastructure.SceneManagement.Signals;
using ForsakenGraves.UnityService.Lobbies;
using MessagePipe;
using VContainer;

namespace ForsakenGraves.Connection.ConnectionStates
{
    public class OfflineState : ConnectionState
    {
        [Inject] private IPublisher<LoadSceneSignal> _sceneLoadPublisher;
        [Inject] private LobbyServiceFacade _lobbyServiceFacade;
        
        public override void Enter()
        {
            _lobbyServiceFacade.EndTracking();

        }

        public override void Exit()
        {
            throw new System.NotImplementedException();
        }
    }
}