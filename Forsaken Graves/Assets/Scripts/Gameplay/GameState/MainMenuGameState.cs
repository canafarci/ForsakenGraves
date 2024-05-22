using System;
using ForsakenGraves.UnityService.Auth;
using ForsakenGraves.UnityService.Lobbies;
using ForsakenGraves.UnityService.Signals;
using MessagePipe;
using Unity.Services.Authentication;
using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace ForsakenGraves.Gameplay.GameState
{
    public class MainMenuGameState : IInitializable
    {
        private readonly AuthenticationServiceFacade _authenticationServiceFacade;
        
        [Inject] private IPublisher<OnAuthenticationSuccessfulSignal> _authorizationSuccessfulPublisher;
        [Inject] private LocalLobbyPlayer _localPlayer;
        
        public MainMenuGameState(AuthenticationServiceFacade authenticationServiceFacade)
        {
            _authenticationServiceFacade = authenticationServiceFacade;
        }
        
        public void Initialize()
        {
            TrySignIn();
        }

        private async void TrySignIn()
        {
            try
            {
                await _authenticationServiceFacade.InitializeAndSignInAsync();
                OnAuthenticationSuccessful();
            }
            catch (Exception e)
            {
                Debug.LogError(e); //TODO show UI
                throw;
            }
        }

        private void OnAuthenticationSuccessful()
        {
            Debug.Log($"Signed in. Unity Player ID {AuthenticationService.Instance.PlayerId}");
            _authorizationSuccessfulPublisher.Publish(new OnAuthenticationSuccessfulSignal());
            _localPlayer.ID = AuthenticationService.Instance.PlayerId;
        }
    }
}