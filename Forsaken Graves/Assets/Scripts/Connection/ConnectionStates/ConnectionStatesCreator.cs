using ForsakenGraves.Infrastructure.Dependencies;
using VContainer;
using VContainer.Unity;

namespace ForsakenGraves.Connection.ConnectionStates
{
    public class ConnectionStatesCreator : IInitializable
    {
        [Inject] private RuntimeInjector _runtimeInjector;
        [Inject] private ConnectionStatesModel _connectionStatesModel;
        [Inject] private ConnectionStateManager _connectionStateManager;
        
        private OfflineState _offlineState;
        private StartingHostState _startingHostState;

        public void Initialize()
        {
            _offlineState = new OfflineState();
            _runtimeInjector.Inject(_offlineState);
            
            //start with offline state
            _connectionStateManager.ChangeState(_offlineState);

            _startingHostState = new StartingHostState();
            _runtimeInjector.Inject(_startingHostState);

            _connectionStatesModel.OfflineState = _offlineState;
            _connectionStatesModel.StartingHostState = _startingHostState;
        }
    }
}