using Unity.Netcode;
using UnityEngine;

namespace ForsakenGraves.Infrastructure.Networking.Data
{
    public struct StatePayload : INetworkSerializable
    {
        public int Tick;
        public Vector3 Position;
        public float YRotation;
            
        public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
        {
            serializer.SerializeValue(ref Tick);
            serializer.SerializeValue(ref Position);
            serializer.SerializeValue(ref YRotation);
        }
    }
}