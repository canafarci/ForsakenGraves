using System;
using System.Collections.Generic;
using ForsakenGraves.UnityService.Data;
using Unity.Services.Lobbies.Models;
using UnityEngine;

namespace ForsakenGraves.UnityService.Lobbies
{
    [Serializable]
    public class LocalLobbyPlayer
    {
        private PlayerData _playerData;
        private UserMembers _lastChanged;
        
        public event Action<LocalLobbyPlayer> OnLobbyPlayerChanged;
        
        public LocalLobbyPlayer()
        {
            _playerData = new PlayerData(isHost: false,
                                         displayName: Guid.NewGuid()
                                                          .ToString()[0..5],
                                         id: null); //TODO change to player options
        }

#region Getters-Setters
        public bool IsHost
        {
            get =>  _playerData.IsHost;
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
        
        public string DisplayName
        {
            get => _playerData.DisplayName;
            set
            {
                if (_playerData.DisplayName != value)
                {
                    _playerData.DisplayName = value;
                    _lastChanged = UserMembers.DisplayName;
                    FireOnLobbyPlayerChangedEvent();
                }
            }
        }
        
        public string ID
        {
            get => _playerData.ID;
            set
            {
                if (_playerData.ID != value)
                {
                    _playerData.ID = value;
                    _lastChanged = UserMembers.ID;
                    FireOnLobbyPlayerChangedEvent();
                }
            }
        }
#endregion

        public void CopyDataFrom(LocalLobbyPlayer player)
        {
            PlayerData data = player._playerData;
            int lastChanged = // Set flags just for the members that will be changed.
                (_playerData.IsHost == data.IsHost ? 0 : (int)UserMembers.IsHost) |
                (_playerData.DisplayName == data.DisplayName ? 0 : (int)UserMembers.DisplayName) |
                (_playerData.ID == data.ID ? 0 : (int)UserMembers.ID);

            if (lastChanged == 0) // Ensure something actually changed.
            {
                return;
            }

            _playerData = data;
            _lastChanged = (UserMembers)lastChanged;

            FireOnLobbyPlayerChangedEvent();
        }
        
        private void FireOnLobbyPlayerChangedEvent()
        {
            OnLobbyPlayerChanged?.Invoke(this);
        }
        
        public Dictionary<string, PlayerDataObject> GetDataForUnityServices()
        {
            return new Dictionary<string, PlayerDataObject>()
            {
                {"DisplayName", new PlayerDataObject(PlayerDataObject.VisibilityOptions.Member, DisplayName)},
            };
        }
        
        public void ResetState() => _playerData = new PlayerData(false, _playerData.DisplayName, _playerData.ID);
        
        [Flags]
        public enum UserMembers
        {
            IsHost = 1,
            DisplayName = 2,
            ID = 4,
        }
    }
}