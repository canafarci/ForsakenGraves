using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using ForsakenGraves.UnityService.Helpers;
using Unity.Services.Authentication;
using Unity.Services.Lobbies;
using Unity.Services.Lobbies.Models;
using UnityEngine;
using VContainer.Unity;

namespace ForsakenGraves.UnityService.Lobbies
{
    public class LobbyServiceFacade : IInitializable
    {
        private readonly LocalLobbyPlayer _localPlayer;

        private LobbyAPIInterface _lobbyApiInterface;
        
        private RateLimitChecker _rateLimitCheckerForHost;
        //https://docs.unity.com/lobby/rate-limits.html
        private const float RATE_LIMIT_FOR_HOST = 3f;

        public LobbyServiceFacade(LocalLobbyPlayer localPlayer)
        {
            _localPlayer = localPlayer;
        }
        
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
                var lobby = await _lobbyApiInterface.CreateLobby(AuthenticationService.Instance.PlayerId, lobbyName,
                                                                 maxConnectedPlayers, 
                                                                 isPrivate,
                                                                 _localPlayer.GetDataForUnityServices(),
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


    }
}