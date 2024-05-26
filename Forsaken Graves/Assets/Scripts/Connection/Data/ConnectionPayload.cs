using System;
using UnityEngine.Serialization;

namespace ForsakenGraves.Connection.Data
{
    [Serializable]
    public class ConnectionPayload
    {
        public string PlayerId;
        public string PlayerName;
    }
}