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
        private ClientConnectingState _clientConnectingState;
        private ClientConnectedState _clientConnectedState;

        public void Start()
        {
            _offlineState = new OfflineState();
            _startingHostState = new StartingHostState();
            _hostingState = new HostingState();
            _clientConnectingState = new ClientConnectingState();
            _clientConnectedState = new ClientConnectedState();
            
            _runtimeInjector.Inject(_startingHostState);
            _runtimeInjector.Inject(_offlineState);
            _runtimeInjector.Inject(_hostingState);
            _runtimeInjector.Inject(_clientConnectingState);
            _runtimeInjector.Inject(_clientConnectedState);

            _connectionStatesModel.OfflineState = _offlineState;
            _connectionStatesModel.StartingHostState = _startingHostState;
            _connectionStatesModel.HostingState = _hostingState;
            _connectionStatesModel.ClientConnectingState = _clientConnectingState;
            _connectionStatesModel.ClientConnectedState = _clientConnectedState;
            
            //start with offline state
            _connectionStateManager.ChangeState(_offlineState);
        }
    }
}