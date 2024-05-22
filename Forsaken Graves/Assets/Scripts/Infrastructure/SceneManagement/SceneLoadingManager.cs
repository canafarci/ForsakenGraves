using System;
using ForsakenGraves.Infrastructure.SceneManagement.Signals;
using MessagePipe;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;
using VContainer;

namespace ForsakenGraves.Infrastructure.SceneManagement
{
    public class SceneLoadingManager : NetworkBehaviour
    {
        [Inject] private ISubscriber<LoadSceneSignal> _sceneLoadSubscriber;
        
        private IDisposable _disposableBag;

        
        private void Awake()
        {
            DontDestroyOnLoad(this);

            SubscribeToMessages();
        }

        private void SubscribeToMessages()
        {
            DisposableBagBuilder bag = DisposableBag.CreateBuilder(); 
            _sceneLoadSubscriber.Subscribe(OnLoadSceneSignal).AddTo(bag);
        }

        private void OnLoadSceneSignal(LoadSceneSignal loadSceneSignal)
        {
            if (SceneManager.GetActiveScene().buildIndex == (int)loadSceneSignal.SceneID) return;
            if (loadSceneSignal.UseNetworkManager)
            {
                throw new Exception("Loading by NetworkManager not implemented");
            }
            else
            {
                // Load using SceneManager
                AsyncOperation loadOperation = SceneManager.LoadSceneAsync((int)loadSceneSignal.SceneID);
                
                //_clientLoadingScreen.StartLoadingScreen(sceneName);
                //_loadingProgressManager.LocalLoadOperation = loadOperation;
            }
            
        }
    }
}