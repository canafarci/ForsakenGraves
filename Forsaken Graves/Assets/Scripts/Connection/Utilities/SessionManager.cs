using System.Collections.Generic;
using UnityEngine;

namespace ForsakenGraves.Connection.Utilities
{
    public class SessionManager<T> where T : struct, ISessionPlayerData
    {
        private Dictionary<ulong, string> _clientIDToPlayerId;
        private Dictionary<string, T> _clientData;
        private bool _hasSessionStarted = false;

        private static SessionManager<T> _instance;
        
        private SessionManager()
        {
            _clientData = new Dictionary<string, T>();
            _clientIDToPlayerId = new Dictionary<ulong, string>();
        }
        
        //singleton getter
        public static SessionManager<T> Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new SessionManager<T>();
                }

                return _instance;
            }
        }
        
        public void SetupConnectingPlayerSessionData(ulong clientId, string playerId, T sessionPlayerData)
        {
            var isReconnecting = false;

            // Test for duplicate connection
            if (IsDuplicateConnection(playerId))
            {
                Debug.LogError($"Player ID {playerId} already exists. This is a duplicate connection. Rejecting this session data.");
                return;
            }

            // If another client exists with the same playerId
            if (_clientData.ContainsKey(playerId))
            {
                if (!_clientData[playerId].IsConnected)
                {
                    // If this connecting client has the same player Id as a disconnected client, this is a reconnection.
                    isReconnecting = true;
                }
            }

            // Reconnecting. Give data from old player to new player
            if (isReconnecting)
            {
                // Update player session data
                sessionPlayerData = _clientData[playerId];
                sessionPlayerData.ClientID = clientId;
                sessionPlayerData.IsConnected = true;
            }

            //Populate our dictionaries with the SessionPlayerData
            _clientIDToPlayerId[clientId] = playerId;
            _clientData[playerId] = sessionPlayerData;
        }
        
        public bool IsDuplicateConnection(string playerId)
        {
            return _clientData.ContainsKey(playerId) && _clientData[playerId].IsConnected;
        }

        public void OnServerEnded()
        {
            _clientData.Clear();
            _clientIDToPlayerId.Clear();
            _hasSessionStarted = false;
        }
    }
}