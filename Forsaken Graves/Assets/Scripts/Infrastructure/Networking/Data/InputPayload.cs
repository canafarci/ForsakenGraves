using System;
using ForsakenGraves.Identifiers;
using Unity.Netcode;
using UnityEngine;

namespace ForsakenGraves.Infrastructure.Networking.Data
{
    public struct InputPayload : INetworkSerializable
    {
        public int Tick;
        public InputFlags Input;
        public float YRotation;
        
        public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
        {
            serializer.SerializeValue(ref Tick);
            serializer.SerializeValue(ref Input);
            serializer.SerializeValue(ref YRotation);
        }
    }
}