using System;
using ForsakenGraves.Infrastructure.SceneManagement.Messages;
using MessagePipe;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;
using VContainer;

namespace ForsakenGraves.Infrastructure.SceneManagement
{
    public class SceneLoadingManager : NetworkBehaviour
    {
        [Inject] private ISubscriber<LoadSceneMessage> _sceneLoadSubscriber;
        
        private IDisposable _disposableBag;

        private bool _isInitialized;

        private void Awake()
        {
            DontDestroyOnLoad(this);

            SubscribeToMessages();
        }
        
        public virtual void Start()
        {
            SceneManager.sceneLoaded += OnSceneLoaded;
            NetworkManager.OnServerStarted += OnNetworkingSessionStarted;
            NetworkManager.OnClientStarted += OnNetworkingSessionStarted;
            NetworkManager.OnServerStopped += OnNetworkingSessionEnded;
            NetworkManager.OnClientStopped += OnNetworkingSessionEnded;
        }
        
        private void OnNetworkingSessionStarted()
        {
            // This prevents this to be called twice on a host, which receives both OnServerStarted and OnClientStarted callbacks
            if (!_isInitialized)
            {
                if (IsNetworkSceneManagementEnabled())
                {
                    NetworkManager.SceneManager.OnSceneEvent += OnSceneEvent;
                }

                _isInitialized = true;
            }
        }
        
        private void OnNetworkingSessionEnded(bool unused)
        {
            if (_isInitialized)
            {
                if (IsNetworkSceneManagementEnabled())
                {
                    NetworkManager.SceneManager.OnSceneEvent -= OnSceneEvent;
                }
                _isInitialized = false;
            }
        }
        
        private void OnSceneLoaded(Scene arg0, LoadSceneMode arg1)
        {
            if (!IsSpawned || NetworkManager.ShutdownInProgress)
            {
                //TODO clear loading screen
            }
        }

        private void OnSceneEvent(SceneEvent sceneEvent)
        {
            SceneEventType eventType = sceneEvent.SceneEventType;

            switch (eventType)
            {
                case SceneEventType.Load: // Server told client to load a scene
                    if (!NetworkManager.IsClient) return; //do not execute if not client or host
                    //TODO Show UI
                    break;
                case SceneEventType.LoadEventCompleted:   // Server told client that all clients finished loading a scene
                    if (!NetworkManager.IsClient) return; //do not execute if not client or host
                    //TODO disable loading UI
                    break;
                case SceneEventType.SynchronizeComplete: // Client told server that they finished synchronizing
                    // Only executes on server
                    if (NetworkManager.IsServer)
                    {
                        // Send client RPC to make sure the client stops the loading screen after the server handles what it needs to after the client finished synchronizing, for example character spawning done server side should still be hidden by loading screen.
                        ClientStopLoadingScreenRpc(RpcTarget.Group(new[] { sceneEvent.ClientId }, RpcTargetUse.Temp));
                    }
                    break;
                default:
                    break;
            }
        }

        private void SubscribeToMessages()
        {
            DisposableBagBuilder bag = DisposableBag.CreateBuilder(); 
            _sceneLoadSubscriber.Subscribe(OnLoadSceneSignal).AddTo(bag);
            
            _disposableBag = bag.Build();
        }

        private void OnLoadSceneSignal(LoadSceneMessage loadSceneMessage)
        {
            if (SceneManager.GetActiveScene().buildIndex == (int)loadSceneMessage.SceneID) return;
            if (loadSceneMessage.UseNetworkManager)
            {
                if (IsSpawned && IsNetworkSceneManagementEnabled() && !NetworkManager.ShutdownInProgress)
                {
                    if (NetworkManager.IsServer)
                    {
                        // If is active server and NetworkManager uses scene management, load scene using NetworkManager's SceneManager
                        NetworkManager.SceneManager.LoadScene(loadSceneMessage.SceneID.ToString(), LoadSceneMode.Single);
                    }
                }
            }
            else
            {
                // Load using SceneManager
                AsyncOperation loadOperation = SceneManager.LoadSceneAsync((int)loadSceneMessage.SceneID);
                
                //_clientLoadingScreen.StartLoadingScreen(sceneName);
                //_loadingProgressManager.LocalLoadOperation = loadOperation;
            }
        }
        
        [Rpc(SendTo.SpecifiedInParams)]
        private void ClientStopLoadingScreenRpc(RpcParams clientRpcParams = default)
        {
            //TODO clear loading screen
        }
        
        private bool IsNetworkSceneManagementEnabled() => NetworkManager != null && NetworkManager.SceneManager != null && NetworkManager.NetworkConfig.EnableSceneManagement;

        public override void OnDestroy()
        {
            SceneManager.sceneLoaded -= OnSceneLoaded;
            
            if (NetworkManager != null)
            {
                NetworkManager.OnServerStarted -= OnNetworkingSessionStarted;
                NetworkManager.OnClientStarted -= OnNetworkingSessionStarted;
                NetworkManager.OnServerStopped -= OnNetworkingSessionEnded;
                NetworkManager.OnClientStopped -= OnNetworkingSessionEnded;
            }
            
            _disposableBag.Dispose();
            base.OnDestroy();
        }
    }
}