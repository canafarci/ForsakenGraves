namespace ForsakenGraves.Connection.ConnectionStates
{
    public class ConnectionStatesModel
    {
        public OfflineState OfflineState { get; set; }
        public StartingHostState StartingHostState { get; set; }
        public HostingState HostingState { get; set; }
        public ClientConnectingState ClientConnectingState { get; set; }
        public ClientConnectedState ClientConnectedState { get; set; }
    }
}