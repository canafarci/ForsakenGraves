using ForsakenGraves.Connection.Identifiers;
using MessagePipe;
using Unity.Netcode;
using VContainer;

namespace ForsakenGraves.Connection.ConnectionStates
{
    public abstract class ConnectionState
    {
        [Inject] protected readonly ConnectionStateManager _connectionStateManager;
        [Inject] protected readonly ConnectionStatesModel _connectionStatesModel;
        [Inject] protected readonly IPublisher<ConnectStatus> _connectStatusPublisher;
        
        public abstract void Enter();
        public abstract void Exit();
        
        public virtual void StartHostLobby(string playerName) { }
        public virtual void StartClientLobby(string playerName) { }
        public virtual void OnClientConnected(ulong clientId) { }
        public virtual void OnClientDisconnect(ulong clientId) { }
        public virtual void OnUserRequestedShutdown() {}
        
        //Network Manager Callbacks
        public virtual void OnServerStopped() { }
        public virtual void OnTransportFailure() { }
        public virtual void ApprovalCheck(NetworkManager.ConnectionApprovalRequest request, NetworkManager.ConnectionApprovalResponse response) { }
        public virtual void OnServerStarted() { }
        public virtual void OnClientDisconnectCallback(ulong clientId) { }
        public virtual void OnClientConnectedCallback(ulong clientId) { }
    }
}