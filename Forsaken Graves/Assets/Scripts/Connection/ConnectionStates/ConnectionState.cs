using Unity.Netcode;
using VContainer;

namespace ForsakenGraves.Connection.ConnectionStates
{
    public abstract class ConnectionState
    {
        [Inject] protected readonly ConnectionStateManager _connectionStateManager;
        
        public abstract void Enter();
        public abstract void Exit();

        public virtual void OnServerStopped() { }
        public virtual void OnTransportFailure() { }
        public virtual void ApprovalCheck(NetworkManager.ConnectionApprovalRequest request, NetworkManager.ConnectionApprovalResponse response) { }
        public virtual void OnServerStarted() { }
        public virtual void OnClientDisconnectCallback(ulong clientId) { }
        public virtual void OnClientConnectedCallback(ulong clientId) { }
    }
}