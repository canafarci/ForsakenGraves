using ForsakenGraves.Application;
using ForsakenGraves.Connection;
using ForsakenGraves.Connection.ConnectionStates;
using ForsakenGraves.Connection.Data;
using ForsakenGraves.Connection.Identifiers;
using ForsakenGraves.Gameplay.Messages;
using ForsakenGraves.Gameplay.Spawners;
using ForsakenGraves.Gameplay.State;
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
            //infrastructure
            builder.RegisterComponent(_sceneLoadingManager);
            builder.RegisterEntryPoint<ApplicationSettings>().AsSelf();
            builder.RegisterEntryPoint<UpdateRunner>().AsSelf();
            builder.Register<RuntimeInjector>(Lifetime.Singleton);
            
            //connection
            builder.RegisterComponent(_networkManager);
            builder.RegisterComponent(_connectionStateManager);
            builder.RegisterEntryPoint<ConnectionStatesCreator>().AsSelf();
            builder.Register<ConnectionStatesModel>(Lifetime.Singleton).AsSelf();
            
            //Unity Services
            builder.Register<AuthenticationServiceFacade>(Lifetime.Singleton);
            builder.Register<ProfileManager>(Lifetime.Singleton);
            
            //persistent Main Menu State
            builder.RegisterEntryPoint<MainMenuState>().AsSelf();
            
            
            //lobby
            builder.RegisterEntryPoint<LobbyServiceFacade>().AsSelf();
            builder.Register<LobbyAPIInterface>(Lifetime.Singleton);
            builder.Register<LocalLobbyPlayer>(Lifetime.Singleton);
            builder.Register<LocalLobby>(Lifetime.Singleton);
            
            //gameplay
            builder.Register<PersistentGameplayState>(Lifetime.Singleton).AsSelf();
            
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
            
            //gameplay
            builder.RegisterMessageBroker<CharacterDiedMessage>(options);
            builder.RegisterMessageBroker<CharacterSpawnedMessage>(options);
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
            SceneManager.LoadScene((int)SceneIdentifier.MainMenu);
        }
    }
}