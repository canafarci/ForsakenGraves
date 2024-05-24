using ForsakenGraves.Identifiers;
using ForsakenGraves.Infrastructure.SceneManagement.Signals;
using MessagePipe;
using VContainer;

namespace ForsakenGraves.Connection.ConnectionStates
{
    public class HostingState : ConnectionState
    {
        [Inject] private IPublisher<LoadSceneSignal> _sceneLoadPublisher;

        public override void Enter()
        {
            //load char select scene when hosting start via networkmanager
            _sceneLoadPublisher.Publish(new LoadSceneSignal(SceneIdentifier.CharSelect, useNetworkManager: true));
        }
        
        public override void Exit() { }
    }
}