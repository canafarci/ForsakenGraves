using System;
using ForsakenGraves.Connection;
using ForsakenGraves.UnityService.Auth;
using ForsakenGraves.UnityService.Lobbies;
using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace ForsakenGraves.Gameplay.UI
{
    public class LobbyCreationMediator : IInitializable, IDisposable
    {
        private readonly LobbyCreationView _view;
        private readonly AuthenticationServiceFacade _authenticationServiceFacade;
        private readonly LobbyServiceFacade _lobbyServiceFacade;
        private readonly ConnectionStateManager _connectionStateManager;

        [Inject] private LocalLobbyPlayer _localLobbyPlayer;
        [Inject] private LocalLobby _localLobby;
        
        public LobbyCreationMediator(LobbyCreationView view, 
                                     AuthenticationServiceFacade authenticationServiceFacade,
                                     LobbyServiceFacade lobbyServiceFacade,
                                     ConnectionStateManager connectionStateManager)
        {
            _view = view;
            _authenticationServiceFacade = authenticationServiceFacade;
            _lobbyServiceFacade = lobbyServiceFacade;
            _connectionStateManager = connectionStateManager;
        }
        
        public void Initialize()
        {
            _view.OnCreateLobbyClicked += View_OnOnCreateLobbyClicked;
        }

        private async void View_OnOnCreateLobbyClicked(object sender, CreateLobbyClickedEventArgs e)
        {
            if (string.IsNullOrEmpty(e.LobbyName)) return;
            
            bool playerIsAuthorized = await _authenticationServiceFacade.EnsurePlayerIsAuthorized();

            if (!playerIsAuthorized) return; //TODO show UI
            
            (bool Success, Unity.Services.Lobbies.Models.Lobby Lobby) lobbyCreationAttempt = await _lobbyServiceFacade.TryCreateLobbyAsync(e.LobbyName, _connectionStateManager.MaxConnectedPlayers, e.IsPrivate);
            
            if (lobbyCreationAttempt.Success)
            {
                _localLobbyPlayer.IsHost = true;
                _lobbyServiceFacade.SetRemoteLobby(lobbyCreationAttempt.Lobby);

                Debug.Log($"Created lobby with ID: {_localLobby.LobbyID} and code {_localLobby.LobbyCode}");
                _connectionStateManager.StartHostLobby(_localLobbyPlayer.DisplayName);
            }
        }
        
        public void Dispose()
        {
            _view.OnCreateLobbyClicked -= View_OnOnCreateLobbyClicked;
        }
    }
}