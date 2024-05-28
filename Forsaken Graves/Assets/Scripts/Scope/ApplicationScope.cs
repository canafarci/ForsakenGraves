using ForsakenGraves.Connection;
using ForsakenGraves.Connection.ConnectionStates;
using ForsakenGraves.Connection.Data;
using ForsakenGraves.Connection.Identifiers;
using ForsakenGraves.GameState;
using ForsakenGraves.Identifiers;
using ForsakenGraves.Infrastructure;
using ForsakenGraves.Infrastructure.Dependencies;
using ForsakenGraves.Infrastructure.Netcode;
using ForsakenGraves.Infrastructure.SceneManagement;
using ForsakenGraves.Infrastructure.SceneManagement.Messages;
using ForsakenGraves.UnityService.Auth;
using ForsakenGraves.UnityService.Lobbies;
using ForsakenGraves.UnityService.Messages;
using MessagePipe;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;
using VContainer;
using VContainer.Unity;

namespace ForsakenGraves.Scope
{
    public class ApplicationScope : LifetimeScope
    {
        [SerializeField] ConnectionStateManager _connectionStateManager;
        [SerializeField] NetworkManager _networkManager;
        [SerializeField] private SceneLoadingManager _sceneLoadingManager;
        
        protected override void Configure(IContainerBuilder builder)
        {
            base.Configure(builder);
            
            builder.RegisterComponent(_connectionStateManager);
            builder.RegisterComponent(_networkManager);
            builder.RegisterComponent(_sceneLoadingManager);
            
            
            builder.RegisterEntryPoint<ConnectionStatesCreator>().AsSelf();
            builder.Register<ConnectionStatesModel>(Lifetime.Singleton).AsSelf();
            
            builder.Register<AuthenticationServiceFacade>(Lifetime.Singleton);
            builder.Register<ProfileManager>(Lifetime.Singleton);
            
            builder.UseEntryPoints(Lifetime.Singleton, entryPoints =>
                                                       {
                                                           entryPoints.Add<MainMenuState>();
                                                       });
            
            builder.RegisterEntryPoint<UpdateRunner>().AsSelf();
            builder.RegisterEntryPoint<LobbyServiceFacade>().AsSelf();
            
            builder.Register<LobbyAPIInterface>(Lifetime.Singleton);
            builder.Register<LocalLobbyPlayer>(Lifetime.Singleton);
            builder.Register<LocalLobby>(Lifetime.Singleton);
            
            builder.Register<RuntimeInjector>(Lifetime.Singleton);
            MessagePipeOptions options = RegisterMessagePipe(builder);
            RegisterMessageBrokers(builder, options);

        }

        private static void RegisterMessageBrokers(IContainerBuilder builder, MessagePipeOptions options)
        {
            builder.RegisterMessageBroker<OnAuthenticationSuccessfulMessage>(options);
            builder.RegisterMessageBroker<LoadSceneMessage>(options);
            builder.RegisterMessageBroker<ConnectionEventMessage>(options);
            builder.RegisterMessageBroker<ConnectStatus>(options);
            
            //netcode
            builder.RegisterMessageBroker<OnNetworkSpawnMessage>(options);
            builder.RegisterMessageBroker<OnNetworkDespawnMessage>(options);
        }

        private MessagePipeOptions RegisterMessagePipe(IContainerBuilder builder)
        {
            MessagePipeOptions options = builder.RegisterMessagePipe();
            builder.RegisterBuildCallback(c => GlobalMessagePipe.SetProvider(c.AsServiceProvider()));
            return options;
        }

        protected override void Awake()
        {
            base.Awake();
            DontDestroyOnLoad(this);
        }

        private void Start()
        {
            Application.targetFrameRate = 120;
            SceneManager.LoadScene((int)SceneIdentifier.MainMenu);
        }
    }
}