using ForsakenGraves.Connection.Utilities;
using ForsakenGraves.Infrastructure.Data;
using UnityEngine;

namespace ForsakenGraves.Connection.Data
{
    public struct SessionPlayerData : ISessionPlayerData
    {
        public string PlayerName { get; set; }
        public bool IsConnected { get; set; }
        public ulong ClientID { get; set; }
        public bool HasCharacterSpawned { get; set; }
        
        public SessionPlayerData(ulong clientID, string name)
        {
            ClientID = clientID;
            PlayerName = name;
            IsConnected = true;
            
            HasCharacterSpawned = false;
        }
        
        public void Reinitialize()
        {
            HasCharacterSpawned = false;
        }
    }
}