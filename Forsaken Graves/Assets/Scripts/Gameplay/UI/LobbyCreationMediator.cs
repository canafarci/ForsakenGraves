using System;
using ForsakenGraves.Connection;
using ForsakenGraves.UnityService.Auth;
using ForsakenGraves.UnityService.Lobbies;
using VContainer.Unity;

namespace ForsakenGraves.Gameplay.UI
{
    public class LobbyCreationMediator : IInitializable, IDisposable
    {
        private readonly LobbyCreationView _view;
        private readonly AuthenticationServiceFacade _authenticationServiceFacade;
        private readonly LobbyServiceFacade _lobbyServiceFacade;
        private readonly ConnectionManager _connectionManager;

        public LobbyCreationMediator(LobbyCreationView view, 
                                     AuthenticationServiceFacade authenticationServiceFacade,
                                     LobbyServiceFacade lobbyServiceFacade,
                                     ConnectionManager connectionManager)
        {
            _view = view;
            _authenticationServiceFacade = authenticationServiceFacade;
            _lobbyServiceFacade = lobbyServiceFacade;
            _connectionManager = connectionManager;
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
            
            var lobbyCreationAttempt = await _lobbyServiceFacade.TryCreateLobbyAsync(e.LobbyName, _connectionManager.MaxConnectedPlayers, e.IsPrivate);

 
        }


        public void Dispose()
        {
            _view.OnCreateLobbyClicked -= View_OnOnCreateLobbyClicked;
        }
    }
}