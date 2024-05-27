using System;
using ForsakenGraves.Connection;
using ForsakenGraves.UnityService.Auth;
using ForsakenGraves.UnityService.Lobbies;
using Unity.Services.Lobbies.Models;
using UnityEngine;
using UnityEngine.Events;
using VContainer;
using VContainer.Unity;

namespace ForsakenGraves.Gameplay.UI
{
    public class LobbyJoiningMediator : IInitializable, IDisposable
    {
        [Inject] private LobbyServiceFacade _lobbyServiceFacade;
        [Inject] private LobbyAPIInterface _lobbyAPIInterface;
        [Inject] private AuthenticationServiceFacade _authenticationServiceFacade;
        [Inject] private LocalLobby _localLobby;
        [Inject] private LocalLobbyPlayer _localLobbyPlayer;
        [Inject] private ConnectionStateManager _connectionStateManager;
        
        private readonly LobbyJoiningView _view;

        public LobbyJoiningMediator(LobbyJoiningView view)
        {
            _view = view;
        }
        
        public void Initialize()
        {
            _view.QuickJoinButton.onClick.AddListener(OnQuickJoinClicked);
        }

        private async void OnQuickJoinClicked()
        {
            bool playerIsAuthorized = await _authenticationServiceFacade.EnsurePlayerIsAuthorized();
            if (!playerIsAuthorized) return; //TODO show UI

            (bool Success, Lobby Lobby) result = await _lobbyServiceFacade.TryQuickJoiningLobbyAsync();

            if (result.Success)
            {
                OnJoinedLobby(result.Lobby);
            }
            else
            {
                //TODO show UI
            }
        }

        private void OnJoinedLobby(Lobby remoteLobby)
        {
            _lobbyServiceFacade.SetRemoteLobby(remoteLobby);
            Debug.Log($"Joined lobby with code: {_localLobby.LobbyCode}, Internal Relay Join Code{_localLobby.RelayJoinCode}");

            _connectionStateManager.StartClientLobby(_localLobbyPlayer.DisplayName);
        }

        public void Dispose()
        {
            _view.QuickJoinButton.onClick.RemoveAllListeners();
        }
    }
}