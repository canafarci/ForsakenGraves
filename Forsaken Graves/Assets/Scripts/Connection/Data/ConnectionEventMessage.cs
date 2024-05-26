using ForsakenGraves.Connection.Identifiers;
using Unity.Netcode;

namespace ForsakenGraves.Connection.Data
{
    public struct ConnectionEventMessage : INetworkSerializeByMemcpy
    {
        public ConnectStatus ConnectStatus;
        public FixedPlayerName PlayerName;
    }
}