using System;
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
        private const float HEARTBEAT_PERIOD = 8;


        private RateLimitChecker _rateLimitCheckerForHost;
        private ILobbyEvents _lobbyEvents;
        
        private Lobby _currentUnityLobby;
        private bool _isTracking = false;
        private float _lastHeartbeatSentTime;
        private LobbyEventConnectionState _lobbyEventConnectionState = LobbyEventConnectionState.Unknown;
        
        public Lobby CurrentUnityLobby => _currentUnityLobby;
        
        public void Initialize()
        {
            _rateLimitCheckerForHost = new RateLimitChecker(RATE_LIMIT_FOR_HOST);
        }
        
        public async UniTask<(bool Success, Lobby Lobby)> TryCreateLobbyAsync(string lobbyName, int maxConnectedPlayers, bool isPrivate)
        {
            if (!_rateLimitCheckerForHost.CanCall)
            {
                Debug.LogWarning("Create Lobby request is over rate limit.");
                return (false, null);
            }
            
            try
            {
                var lobby = await _lobbyApiInterface.CreateLobby(AuthenticationService.Instance.PlayerId,
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
                    _rateLimitCheckerForHost.PutOnCooldown();
                }
                else
                {
                    Debug.LogError(e); //TODO show UI
                    throw;
                }
            }

            return (false, null);
        }
        
        public void SetRemoteLobby(Lobby lobby)
        {
            _currentUnityLobby = lobby;
            _localLobby.ApplyRemoteData(lobby);
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
                    _updateRunner.Unsubscribe(DoLobbyHeartbeat);
                }
            }

            if (CurrentUnityLobby != null)
            {
                
            }
        }

        private async void UnsubscribeToJoinedLobbyAsync()
        {
            if (_lobbyEvents != null && _lobbyEventConnectionState != LobbyEventConnectionState.Unsubscribed)
            {
                await _lobbyEvents.UnsubscribeAsync();
            }
        }
        
        void DoLobbyHeartbeat(float dt)
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


        public void Dispose()
        {
            EndTracking();
        }
    }
}