using System;
using ForsakenGraves.Connection;
using ForsakenGraves.Connection.ConnectionStates;
using ForsakenGraves.Connection.Identifiers;
using ForsakenGraves.Gameplay.GameState;
using ForsakenGraves.Identifiers;
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
using UnityEngine.SceneManagement;
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
            
            
            builder.RegisterEntryPoint<ConnectionStatesCreator>().AsSelf();
            builder.Register<ConnectionStatesModel>(Lifetime.Singleton);
            
            builder.Register<RuntimeInjector>(Lifetime.Singleton);
            builder.Register<AuthenticationServiceFacade>(Lifetime.Singleton);
            builder.Register<ProfileManager>(Lifetime.Singleton);
            
            builder.UseEntryPoints(Lifetime.Singleton, entryPoints =>
                                                       {
                                                           entryPoints.Add<MainMenuGameState>();
                                                       });
            
            builder.RegisterEntryPoint<UpdateRunner>().AsSelf();
            builder.RegisterEntryPoint<LobbyServiceFacade>().AsSelf();
            
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
            builder.RegisterMessageBroker<ConnectStatus>(options);
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