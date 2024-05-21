using System;
using System.Collections.Generic;
using Unity.Services.Lobbies.Models;
using UnityEngine;

namespace ForsakenGraves.UnityService.Lobbies
{
    public class LocalLobbyPlayer
    {
        private PlayerData _playerData;
        private UserMembers _lastChanged;
        
        public event Action<LocalLobbyPlayer> OnLobbyPlayerChanged;
        
        public LocalLobbyPlayer()
        {
            _playerData = new PlayerData(isHost: false, displayName: null, id: null);
        }
        
        public struct PlayerData
        {
            public bool IsHost { get; set; }
            public string DisplayName { get; set; }
            public string ID { get; set; }

            public PlayerData(bool isHost, string displayName, string id)
            {
                IsHost = isHost;
                DisplayName = displayName;
                ID = id;
            }
        }
        
        public bool IsHost
        {
            get { return _playerData.IsHost; }
            set
            {
                if (_playerData.IsHost != value)
                {
                    _playerData.IsHost = value;
                    _lastChanged = UserMembers.IsHost;
                    FireOnLobbyPlayerChangedEvent();
                }
            }
        }
        
        private void FireOnLobbyPlayerChangedEvent()
        {
            OnLobbyPlayerChanged?.Invoke(this);
        }
        
        public Dictionary<string, PlayerDataObject> GetDataForUnityServices()
        {
            throw new NotImplementedException();
        }
        
        [Flags]
        public enum UserMembers
        {
            IsHost = 1,
            DisplayName = 2,
            ID = 4,
        }
    }
}