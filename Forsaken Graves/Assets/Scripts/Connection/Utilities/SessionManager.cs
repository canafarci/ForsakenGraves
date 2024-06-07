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
            bool isReconnecting = false;

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

        public T? GetPlayerData(ulong clientId)
        {
            string playerID = GetPlayerID(clientId);
            
            if (playerID == null)
            {
                Debug.LogError($"No data associated with {clientId} has been found!");
                return null;
            }
            else
            {
                return GetPlayerData(playerID);
            }
        }

        public T? GetPlayerData(string playerID)
        {
            if (!_clientData.ContainsKey(playerID))
            {
                Debug.LogError($"No player data associated with {playerID} has been found!");
                return null;
            }
            else
            {
                return _clientData[playerID];
            }
        }

        public string GetPlayerID(ulong clientId)
        {
            if (_clientIDToPlayerId.ContainsKey(clientId))
            {
                return _clientIDToPlayerId[clientId];
            }
            else
            {
                return null;
            }
        }

        public void DisconnectClient(ulong clientId)
        {
            if (_hasSessionStarted)
            {
                // Mark client as disconnected, but keep their data so they can reconnect.
                if (_clientIDToPlayerId.TryGetValue(clientId, out string playerId))
                {
                    T? playerData = GetPlayerData(playerId);
                    if (playerData != null && playerData.Value.ClientID == clientId)
                    {
                        T clientData = _clientData[playerId];
                        clientData.IsConnected = false;
                        _clientData[playerId] = clientData;
                    }
                }
            }
            else
            {
                // Session has not started, no need to keep their data
                if (_clientIDToPlayerId.TryGetValue(clientId, out string playerId))
                {
                    _clientIDToPlayerId.Remove(clientId);
                    T? playerData = GetPlayerData(playerId);
                    if (playerData != null && playerData.Value.ClientID == clientId)
                    {
                        _clientData.Remove(playerId);
                    }
                }
            }
        }

        public void OnSessionEnded()
        {
            ClearDisconnectedPlayersData();
            ReinitializePlayersData();
            
            _hasSessionStarted = false;
        }
        
        private void ReinitializePlayersData()
        {
            foreach (ulong id in _clientIDToPlayerId.Keys)
            {
                string playerId = _clientIDToPlayerId[id];
                T sessionPlayerData = _clientData[playerId];
                sessionPlayerData.Reinitialize();
                _clientData[playerId] = sessionPlayerData;
            }
        }
        
        private void ClearDisconnectedPlayersData()
        {
            List<ulong> idsToClear = new List<ulong>();
            
            foreach (ulong id in _clientIDToPlayerId.Keys)
            {
                T? data = GetPlayerData(id);
                if (data is { IsConnected: false })
                {
                    idsToClear.Add(id);
                }
            }

            foreach (ulong id in idsToClear)
            {
                string playerId = _clientIDToPlayerId[id];
                T? playerData = GetPlayerData(playerId);
                if (playerData != null && playerData.Value.ClientID == id)
                {
                    _clientData.Remove(playerId);
                }

                _clientIDToPlayerId.Remove(id);
            }
        }
    }
}