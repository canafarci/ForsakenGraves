using ForsakenGraves.Connection.Identifiers;

namespace ForsakenGraves.Connection.ConnectionStates
{
    public abstract class OnlineState : ConnectionState
    {
        public override void OnUserRequestedShutdown()
        {
            // This behaviour will be the same for every online state
            _connectStatusPublisher.Publish(ConnectStatus.UserRequestedDisconnect);
            
            OfflineState offlineState = _connectionStatesModel.OfflineState;
            _connectionStateManager.ChangeState(offlineState);
        }

        public override void OnTransportFailure()
        {
            // This behaviour will be the same for every online state
            OfflineState offlineState = _connectionStatesModel.OfflineState;
            _connectionStateManager.ChangeState(offlineState);
        }
    }
}