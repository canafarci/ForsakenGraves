using ForsakenGraves.UnityService.Auth;
using VContainer.Unity;

namespace ForsakenGraves.Gameplay.GameState
{
    public class MainMenuGameState : IInitializable
    {
        private readonly AuthenticationServiceFacade _authenticationServiceFacade;

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
            await _authenticationServiceFacade.InitializeAndSignInAsync();
        }
    }
}