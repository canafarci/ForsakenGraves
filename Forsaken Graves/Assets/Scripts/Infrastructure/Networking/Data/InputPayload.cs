using System;
using ForsakenGraves.Identifiers;
using Unity.Netcode;
using UnityEngine;

namespace ForsakenGraves.Infrastructure.Networking.Data
{
    public struct InputPayload : INetworkSerializable
    {
        public int Tick;
        public Vector3 InputVector;
        
        public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
        {
            serializer.SerializeValue(ref Tick);
            serializer.SerializeValue(ref InputVector);
        }
    }
}