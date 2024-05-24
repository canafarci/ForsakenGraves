using ForsakenGraves.Infrastructure.Dependencies;
using VContainer;
using VContainer.Unity;

namespace ForsakenGraves.Connection.ConnectionStates
{
    public class ConnectionStatesCreator : IStartable
    {
        [Inject] private RuntimeInjector _runtimeInjector;
        [Inject] private ConnectionStatesModel _connectionStatesModel;
        [Inject] private ConnectionStateManager _connectionStateManager;
        
        private OfflineState _offlineState;
        private StartingHostState _startingHostState;
        private HostingState _hostingState;

        public void Start()
        {
            _offlineState = new OfflineState();
            _startingHostState = new StartingHostState();
            _hostingState = new HostingState();
            
            _runtimeInjector.Inject(_startingHostState);
            _runtimeInjector.Inject(_offlineState);
            _runtimeInjector.Inject(_hostingState);

            _connectionStatesModel.OfflineState = _offlineState;
            _connectionStatesModel.StartingHostState = _startingHostState;
            _connectionStatesModel.HostingState = _hostingState;
            
            //start with offline state
            _connectionStateManager.ChangeState(_offlineState);
        }
    }
}