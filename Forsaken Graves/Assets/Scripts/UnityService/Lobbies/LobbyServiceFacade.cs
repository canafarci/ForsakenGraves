using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using ForsakenGraves.Infrastructure;
using ForsakenGraves.UnityService.Helpers;
using Unity.Services.Authentication;
using Unity.Services.Lobbies;
using Unity.Services.Lobbies.Models;
using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace ForsakenGraves.UnityService.Lobbies
{
    public class LobbyServiceFacade : IInitializable, IDisposable
    {
        [Inject] private LocalLobbyPlayer _localLobbyPlayer;
        [Inject] private LocalLobby _localLobby;
        [Inject] private LobbyAPIInterface _lobbyApiInterface;
        [Inject] private UpdateRunner _updateRunner;
        
        //https://docs.unity.com/lobby/rate-limits.html
        private const float RATE_LIMIT_FOR_HOST = 3f;
        private const float RATE_LIMIT_FOR_QUERY = 1f;
        private const float RATE_LIMIT_FOR_QUICK_JOIN = 10f;
        private const float HEARTBEAT_PERIOD = 8;
        
        private RateLimitChecker _hostRateLimitChecker;
        private RateLimitChecker _queryRateLimitChecker;
        private RateLimitChecker _quickJoinRateLimitChecker;
        
        private ILobbyEvents _lobbyEvents;
        
        private Lobby _currentUnityLobby;
        private LobbyEventConnectionState _lobbyEventConnectionState = LobbyEventConnectionState.Unknown;
        private bool _isTracking = false;
        private float _lastHeartbeatSentTime = 0f;
        
        public Lobby CurrentUnityLobby => _currentUnityLobby;
        
        public void Initialize()
        {
            _hostRateLimitChecker = new RateLimitChecker(RATE_LIMIT_FOR_HOST);
            _queryRateLimitChecker = new RateLimitChecker(RATE_LIMIT_FOR_QUERY);
            _quickJoinRateLimitChecker = new RateLimitChecker(RATE_LIMIT_FOR_QUICK_JOIN);
        }
        
#region Create Lobby
        public async UniTask<(bool Success, Lobby Lobby)> TryCreateLobbyAsync(string lobbyName, int maxConnectedPlayers, bool isPrivate)
        {
            if (!_hostRateLimitChecker.CanCall)
            {
                Debug.LogWarning("Create Lobby request is over rate limit.");
                return (false, null);
            }
            
            try
            {
                Lobby lobby = await _lobbyApiInterface.CreateLobby(AuthenticationService.Instance.PlayerId,
                                                                   lobbyName,
                                                                   maxConnectedPlayers, 
                                                                   isPrivate,
                                                                   _localLobbyPlayer.GetDataForUnityServices(),
                                                                   null);
                return (true, lobby);
            }
            catch (LobbyServiceException e)
            {
                if (e.Reason == LobbyExceptionReason.RateLimited)
                {
                    _hostRateLimitChecker.PutOnCooldown();
                }
                else
                {
                    Debug.LogError(e); //TODO show UI
                    throw;
                }
            }

            return (false, null);
        }
        
        public async UniTask UpdateLobbyDataAndUnlockAsync()
        {
            if (!_queryRateLimitChecker.CanCall) return;

            Dictionary<string, DataObject> localData = _localLobby.GetDataForUnityServices();
            Dictionary<string, DataObject> currentData = _currentUnityLobby.Data ?? new();
            
            foreach (KeyValuePair<string, DataObject> newData in localData)
            {
                if (currentData.ContainsKey(newData.Key))
                {
                    currentData[newData.Key] = newData.Value;
                }
                else
                {
                    currentData.Add(newData.Key, newData.Value);
                }
            }
            
            try
            {
                Lobby result = await _lobbyApiInterface.UpdateLobby(_currentUnityLobby.Id, currentData, shouldLock: false);
                
                if (result != null)
                {
                    _currentUnityLobby = result;
                }
            }
            catch (LobbyServiceException e)
            {
                if (e.Reason == LobbyExceptionReason.RateLimited)
                {
                    _queryRateLimitChecker.PutOnCooldown();
                }
                else
                {
                    Debug.LogError(e); //TODO show UI
                }
            }
        }
        
        public async UniTask UpdatePlayerDataAsync(string allocationId, string connectionInfo)
        {
            if (!_queryRateLimitChecker.CanCall) return;

            try
            {
                Lobby result = await _lobbyApiInterface.UpdatePlayer(_currentUnityLobby.Id,
                                                                     AuthenticationService.Instance.PlayerId,
                                                                     _localLobbyPlayer.GetDataForUnityServices(),
                                                                     allocationId,
                                                                     connectionInfo);

            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }
        
        public void SetRemoteLobby(Lobby lobby)
        {
            _currentUnityLobby = lobby;
            _localLobby.ApplyRemoteData(lobby);
        }
#endregion

#region Join Lobby
        public async UniTask<(bool Success, Lobby Lobby)> TryQuickJoiningLobbyAsync()
        {
            if (!_quickJoinRateLimitChecker.CanCall)
            {
                Debug.LogWarning("Quick Join Lobby hit the rate limit.");
                return (false, null);
            }

            try
            {
                Lobby lobby = await _lobbyApiInterface.QuickJoinLobby(AuthenticationService.Instance.PlayerId,
                                                        _localLobbyPlayer.GetDataForUnityServices());
                return (true, lobby);
            }
            catch (LobbyServiceException e)
            {
                if (e.Reason == LobbyExceptionReason.RateLimited)
                {
                    _quickJoinRateLimitChecker.PutOnCooldown();
                }
                else
                {
                    Debug.LogWarning(e);
                    //TODO show UI
                }
            }
            
            return (false, null);

        }
#endregion

#region Lobby Events
        private async void SubscribeToJoinedLobbyAsync()
        {
            LobbyEventCallbacks lobbyEventCallbacks = new LobbyEventCallbacks();
            lobbyEventCallbacks.LobbyChanged += OnLobbyChanges;
            lobbyEventCallbacks.KickedFromLobby += OnKickedFromLobby;
            lobbyEventCallbacks.LobbyEventConnectionStateChanged += OnLobbyEventConnectionStateChanged;
            // The LobbyEventCallbacks object created here will now be managed by the Lobby SDK. The callbacks will be
            // unsubscribed from when we call UnsubscribeAsync on the ILobbyEvents object we receive and store here.
            _lobbyEvents = await _lobbyApiInterface.SubscribeToLobby(_localLobby.LobbyID, lobbyEventCallbacks);
        }
        
        private void OnLobbyChanges(ILobbyChanges changes)
        {
            if (changes.LobbyDeleted)
            {
                ResetLobby();
                EndTracking();
            }
            else //lobby updated
            {
                changes.ApplyToLobby(_currentUnityLobby);
                _localLobby.ApplyRemoteData(_currentUnityLobby);

                bool hostIsInLobby = _localLobbyPlayer.IsHost;
                if (!_localLobbyPlayer.IsHost) //if not host, check if host is still in the lobby
                {
                    foreach (var lobbyUser in _localLobby.LobbyPlayers)
                    {
                        if (lobbyUser.Value.IsHost)
                        {
                            hostIsInLobby = true;
                            break;
                        }
                    }
                }

                if (!hostIsInLobby)
                {
                    Debug.LogWarning("HOST HAS LEFT THE LOBBY");
                    //TODO SHOW UI
                    EndTracking();
                    //netcode auto disconnects here
                }
            }
        }
        
        private void OnKickedFromLobby()
        {
            //TODO SHOW UI
            ResetLobby();
            EndTracking();
        }
        
        private void OnLobbyEventConnectionStateChanged(LobbyEventConnectionState lobbyEventConnectionState)
        {
            _lobbyEventConnectionState = lobbyEventConnectionState;
            Debug.Log($"LobbyEventConnectionState changed to {lobbyEventConnectionState}");
        }
        
        private async void UnsubscribeToJoinedLobbyAsync()
        {
            if (_lobbyEvents != null && _lobbyEventConnectionState != LobbyEventConnectionState.Unsubscribed)
            {
                await _lobbyEvents.UnsubscribeAsync();
            }
        }
#endregion

#region Tracking
        public void BeginTracking()
        {
            if (!_isTracking)
            {
                _isTracking = true;
                SubscribeToJoinedLobbyAsync();

                if (_localLobbyPlayer.IsHost) //send lobby heartbeat if host
                {
                    _lastHeartbeatSentTime = 0f;
                    _updateRunner.Subscribe(SendLobbyHeartbeat, 1.5f);
                }
            }
        }

        public void EndTracking()
        {
            if (_isTracking)
            {
                _isTracking = false;
                UnsubscribeToJoinedLobbyAsync();
                        
                // Only the host sends heartbeat pings to the service to keep the lobby alive
                if (_localLobbyPlayer.IsHost)
                {
                    _updateRunner.Unsubscribe(SendLobbyHeartbeat);
                }
            }

            if (CurrentUnityLobby != null)
            {
                if (_localLobbyPlayer.IsHost)
                {
                    DeleteLobbyAsync();
                }
                else
                {
                    LeaveLobbyAsync();
                }
            }
        }
        
        private void SendLobbyHeartbeat(float dt)
        {
            _lastHeartbeatSentTime += dt;
            if (_lastHeartbeatSentTime > HEARTBEAT_PERIOD)
            {
                _lastHeartbeatSentTime -= HEARTBEAT_PERIOD;
                try
                {
                    _lobbyApiInterface.SendHeartbeatPing(CurrentUnityLobby.Id);
                }
                catch (LobbyServiceException e)
                {
                    // If Lobby is not found and if we are not the host, it has already been deleted. No need to publish the error here.
                    if (e.Reason != LobbyExceptionReason.LobbyNotFound && !_localLobbyPlayer.IsHost)
                    {
                        Debug.LogError(e); //TODO show UI
                    }
                }
            }
        }
#endregion
        
#region Leave Lobby
        private async void LeaveLobbyAsync()
        {
            string serviceID = AuthenticationService.Instance.PlayerId;
            
            try
            {
                await _lobbyApiInterface.RemovePlayerFromLobby(serviceID, _localLobby.LobbyID);
            }
            catch (LobbyServiceException e)
            {
                // If Lobby is not found and if we are not the host, it has already been deleted. No need to publish the error here.
                if (e.Reason != LobbyExceptionReason.LobbyNotFound && !_localLobbyPlayer.IsHost)
                {
                    Debug.LogError(e); //TODO show error
                }
            }
            finally
            {
                ResetLobby();
            }
        }
        
        public async void RemovePlayerFromLobbyAsync(string authenticationID)
        {
            if (_localLobbyPlayer.IsHost)
            {
                _lobbyApiInterface.RemovePlayerFromLobby(authenticationID, _localLobby.LobbyID);
            }
            else
            {
                Debug.LogError("Only host can remove players from lobby");
            }
        }

        private async void DeleteLobbyAsync()
        {
            if (_localLobbyPlayer.IsHost)
            {
                try
                {
                    await _lobbyApiInterface.DeleteLobby(_localLobby.LobbyID);
                }
                catch (LobbyServiceException e)
                {
                    Debug.LogError(e); //TODO show error
                }
                finally
                {
                    ResetLobby();
                }
            }
            else
            {
                Debug.LogError("Only the host can delete a lobby.");
            }
        }

        private void ResetLobby()
        {
            _currentUnityLobby = null;
            
            if (_localLobbyPlayer != null)
            {
                _localLobbyPlayer.ResetState();
            }
            if (_localLobby != null)
            {
                _localLobby.Reset(_localLobbyPlayer);
            }
        }
#endregion

        public void Dispose()
        {
            EndTracking();
        }
    }
}