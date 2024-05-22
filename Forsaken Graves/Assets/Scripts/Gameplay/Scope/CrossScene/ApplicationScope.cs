using ForsakenGraves.Connection;
using ForsakenGraves.Gameplay.GameState;
using ForsakenGraves.Infrastructure;
using ForsakenGraves.Infrastructure.Dependencies;
using ForsakenGraves.Infrastructure.SceneManagement;
using ForsakenGraves.Infrastructure.SceneManagement.Signals;
using ForsakenGraves.UnityService.Auth;
using ForsakenGraves.UnityService.Lobbies;
using ForsakenGraves.UnityService.Signals;
using MessagePipe;
using Unity.Netcode;
using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace ForsakenGraves.Gameplay.Scope.CrossScene
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
            
            builder.Register<RuntimeInjector>(Lifetime.Singleton);
            
            builder.Register<AuthenticationServiceFacade>(Lifetime.Singleton);
            builder.Register<ProfileManager>(Lifetime.Singleton);
            
            builder.UseEntryPoints(Lifetime.Singleton, entryPoints =>
                                                       {
                                                           entryPoints.Add<MainMenuGameState>();
                                                           entryPoints.Add<UpdateRunner>();
                                                       });
            
            builder.Register<LobbyAPIInterface>(Lifetime.Singleton);
            builder.Register<LocalLobbyPlayer>(Lifetime.Singleton);
            builder.Register<LocalLobby>(Lifetime.Singleton);
            
            //builder.RegisterEntryPoint<UpdateRunner>().AsSelf();


            MessagePipeOptions options = RegisterMessagePipe(builder);
            RegisterMessageBrokers(builder, options);

        }

        private static void RegisterMessageBrokers(IContainerBuilder builder, MessagePipeOptions options)
        {
            builder.RegisterMessageBroker<OnAuthenticationSuccessfulSignal>(options);
            builder.RegisterMessageBroker<LoadSceneSignal>(options);
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
    }
}