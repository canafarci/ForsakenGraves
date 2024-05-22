using System;
using System.Collections.Generic;
using ForsakenGraves.UnityService.Data;
using Unity.Services.Lobbies.Models;
using UnityEngine;

namespace ForsakenGraves.UnityService.Lobbies
{
    [Serializable]
    public class LocalLobby
    {
        private Dictionary<string, LocalLobbyPlayer> _lobbyPlayers = new Dictionary<string, LocalLobbyPlayer>();
        private LobbyData _lobbyData;

#region Getter-Setters

        public int PlayerCount => _lobbyPlayers.Count;

        public LobbyData LobbyData => new LobbyData(_lobbyData);
        public Dictionary<string, LocalLobbyPlayer> LobbyPlayers => _lobbyPlayers;
        
        public string LobbyID
        {
            get => _lobbyData.LobbyID;
            set
            {
                _lobbyData.LobbyID = value;
                FireOnLocalLobbyChangedEvent();
            }
        }

        public string LobbyCode
        {
            get => _lobbyData.LobbyCode;
            set
            {
                _lobbyData.LobbyCode = value;
                FireOnLocalLobbyChangedEvent();
            }
        }

        public string RelayJoinCode
        {
            get => _lobbyData.RelayJoinCode;
            set
            {
                _lobbyData.RelayJoinCode = value;
                FireOnLocalLobbyChangedEvent();
            }
        }

        public string LobbyName
        {
            get => _lobbyData.LobbyName;
            set
            {
                _lobbyData.LobbyName = value;
                FireOnLocalLobbyChangedEvent();
            }
        }

        public bool Private
        {
            get => _lobbyData.Private;
            set
            {
                _lobbyData.Private = value;
                FireOnLocalLobbyChangedEvent();
            }
        }
        
        public int MaxPlayerCount
        {
            get => _lobbyData.MaxPlayerCount;
            set
            {
                _lobbyData.MaxPlayerCount = value;
                FireOnLocalLobbyChangedEvent();
            }
        }
#endregion        
        
        public event Action<LocalLobby> OnLocalLobbyChanged;
  
        public void ApplyRemoteData(Lobby lobby)
        {
            LobbyData info = new LobbyData();
            info.LobbyID = lobby.Id;
            info.LobbyCode = lobby.LobbyCode;
            info.Private = lobby.IsPrivate;
            info.LobbyName = lobby.Name;
            info.MaxPlayerCount = lobby.MaxPlayers;
            
            if (lobby.Data != null)
            {
                // By providing RelayCode through the lobby data with Member visibility, we ensure a client is connected to the lobby before they could attempt a relay connection, preventing timing issues between them.
                info.RelayJoinCode = lobby.Data.ContainsKey("RelayJoinCode") ? lobby.Data["RelayJoinCode"].Value : null; 
            }
            else
            {
                info.RelayJoinCode = null;
            }
            
            Dictionary<string, LocalLobbyPlayer> lobbyPlayers = new Dictionary<string, LocalLobbyPlayer>();
            foreach (var player in lobby.Players)
            {
                if (player.Data != null)
                {
                    if (_lobbyPlayers.ContainsKey(player.Id))
                    {
                        lobbyPlayers.Add(player.Id, _lobbyPlayers[player.Id]);
                        continue;
                    }
                }

                // If the player isn't connected to Relay, get the most recent data that the lobby knows.
                // (If we haven't seen this player yet, a new local representation of the player will have already been added by the LocalLobby.)
                var incomingData = new LocalLobbyPlayer()
                                   {
                                       IsHost = lobby.HostId.Equals(player.Id),
                                       DisplayName = player.Data != null && player.Data.ContainsKey("DisplayName") ? player.Data["DisplayName"].Value : default,
                                       ID = player.Id
                                   };

                lobbyPlayers.Add(incomingData.ID, incomingData);
            }
            
            CopyDataFrom(info, lobbyPlayers);
        }
        
        public void CopyDataFrom(LobbyData data, Dictionary<string, LocalLobbyPlayer> currUsers)
        {
            _lobbyData = data;

            if (currUsers == null)
            {
                _lobbyPlayers = new Dictionary<string, LocalLobbyPlayer>();
            }
            else
            {
                List<LocalLobbyPlayer> toRemove = new List<LocalLobbyPlayer>();
                foreach (var oldUser in _lobbyPlayers)
                {
                    if (currUsers.ContainsKey(oldUser.Key))
                    {
                        oldUser.Value.CopyDataFrom(currUsers[oldUser.Key]);
                    }
                    else
                    {
                        toRemove.Add(oldUser.Value);
                    }
                }

                foreach (var remove in toRemove)
                {
                    DoRemoveUser(remove);
                }

                foreach (var currUser in currUsers)
                {
                    if (!_lobbyPlayers.ContainsKey(currUser.Key))
                    {
                        DoAddUser(currUser.Value);
                    }
                }
            }

            FireOnLocalLobbyChangedEvent();
        }
        
        private void DoAddUser(LocalLobbyPlayer player)
        {
            _lobbyPlayers.Add(player.ID, player);
            player.OnLobbyPlayerChanged += OnChangedUser;
        }
        
        private void DoRemoveUser(LocalLobbyPlayer player)
        {
            if (!_lobbyPlayers.ContainsKey(player.ID))
            {
                Debug.LogWarning($"Player {player.DisplayName}({player.ID}) does not exist in lobby: {LobbyID}");
                return;
            }

            _lobbyPlayers.Remove(player.ID);
            player.OnLobbyPlayerChanged -= OnChangedUser;
        }

        private void OnChangedUser(LocalLobbyPlayer player)
        {
            FireOnLocalLobbyChangedEvent();
        }

        private void FireOnLocalLobbyChangedEvent()
        {
            OnLocalLobbyChanged?.Invoke(this);
        }
    }
}