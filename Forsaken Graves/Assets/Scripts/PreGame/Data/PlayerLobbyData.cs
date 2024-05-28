using System;
using Unity.Netcode;

namespace ForsakenGraves.PreGame.Data
{
    public struct PlayerLobbyData : INetworkSerializable, IEquatable<PlayerLobbyData>
    {
        public bool IsReady;
        public int AvatarIndex;
        public ulong ClientID;

        public PlayerLobbyData(ulong clientID, bool isReady = false, int avatarIndex = 0)
        {
            ClientID = clientID;
            IsReady = isReady;
            AvatarIndex = avatarIndex;
        }
        
        public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
        {
            serializer.SerializeValue(ref IsReady);
            serializer.SerializeValue(ref AvatarIndex);
            serializer.SerializeValue(ref ClientID);
        }

        public bool Equals(PlayerLobbyData other)
        {
            return IsReady == other.IsReady &&
                   AvatarIndex == other.AvatarIndex &&
                   ClientID == other.ClientID;
        }
    }
}