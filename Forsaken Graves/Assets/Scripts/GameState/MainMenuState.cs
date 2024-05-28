using System;
using ForsakenGraves.UnityService.Auth;
using ForsakenGraves.UnityService.Lobbies;
using ForsakenGraves.UnityService.Messages;
using MessagePipe;
using Unity.Services.Authentication;
using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace ForsakenGraves.GameState
{
    public class MainMenuState : IInitializable
    {
        private readonly AuthenticationServiceFacade _authenticationServiceFacade;
        
        [Inject] private IPublisher<OnAuthenticationSuccessfulMessage> _authorizationSuccessfulPublisher;
        [Inject] private LocalLobbyPlayer _localPlayer;
        
        public MainMenuState(AuthenticationServiceFacade authenticationServiceFacade)
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
            _authorizationSuccessfulPublisher.Publish(new OnAuthenticationSuccessfulMessage());
            _localPlayer.ID = AuthenticationService.Instance.PlayerId;
        }
    }
}