using System;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using Unity.Services.Authentication;
using Unity.Services.Core;

namespace ForsakenGraves.UnityService.Auth
{
    public class AuthenticationServiceFacade 
    {
        public async UniTask InitializeAndSignInAsync()
        {
            if (UnityServices.State != ServicesInitializationState.Initialized)
            {
                InitializationOptions options = new();
                string uniqueID = CreateUniqueID();

                options.SetProfile(uniqueID);

                await UnityServices.InitializeAsync(options);
                await AuthenticationService.Instance.SignInAnonymouslyAsync();
            }
        }
        
        private static string CreateUniqueID()
        {
            Guid uniqueID = Guid.NewGuid();
            string idText = uniqueID.ToString();
            //authentication service requires names to be 30 charaters long and GUIDs are 36 characters,
            //so remove the last 6 characters
            string slicedID = idText[..^6];
            return slicedID;
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