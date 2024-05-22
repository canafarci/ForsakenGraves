using System;
using Cysharp.Threading.Tasks;
using ForsakenGraves.Utility;
using Unity.Services.Authentication;
using Unity.Services.Core;

namespace ForsakenGraves.UnityService.Auth
{
    public class AuthenticationServiceFacade 
    {
        private readonly ProfileManager _profileManager;

        public AuthenticationServiceFacade(ProfileManager profileManager)
        {
            _profileManager = profileManager;
        }
        
        public async UniTask InitializeAndSignInAsync()
        {
            if (UnityServices.State != ServicesInitializationState.Initialized)
            {
                InitializationOptions options = new();
                string uniqueID =  _profileManager.GetUniqueProfileID();

                options.SetProfile(uniqueID);

                await UnityServices.InitializeAsync(options);
                await AuthenticationService.Instance.SignInAnonymouslyAsync();
            }
        }
        
        public async UniTask<bool> EnsurePlayerIsAuthorized()
        {
            bool isAuthorized = false;
            
            if (AuthenticationService.Instance.IsAuthorized)
            {
                isAuthorized = true;
            }
            else
            {
                try
                {
                    await AuthenticationService.Instance.SignInAnonymouslyAsync();
                    isAuthorized = true;
                }
                catch (AuthenticationException e)
                {
                    Console.WriteLine(e);
                    isAuthorized = false;
                    throw;
                }
            }
            
            return isAuthorized;
        }
    }
}