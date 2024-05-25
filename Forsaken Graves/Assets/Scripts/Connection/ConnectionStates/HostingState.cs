using ForsakenGraves.Connection.Data;
using ForsakenGraves.Connection.Utilities;
using ForsakenGraves.Identifiers;
using ForsakenGraves.Infrastructure.SceneManagement.Signals;
using ForsakenGraves.UnityService.Lobbies;
using MessagePipe;
using VContainer;

namespace ForsakenGraves.Connection.ConnectionStates
{
    public class HostingState : ConnectionState
    {
        [Inject] private IPublisher<LoadSceneSignal> _sceneLoadPublisher;
        [Inject] private LobbyServiceFacade _lobbyServiceFacade;
        
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
    }
}