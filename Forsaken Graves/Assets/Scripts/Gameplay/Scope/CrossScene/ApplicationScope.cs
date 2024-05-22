using ForsakenGraves.Connection;
using ForsakenGraves.Gameplay.GameState;
using ForsakenGraves.UnityService.Auth;
using ForsakenGraves.UnityService.Lobbies;
using ForsakenGraves.UnityService.Signals;
using ForsakenGraves.Utility;
using MessagePipe;
using UnityEngine;
using UnityEngine.Serialization;
using VContainer;
using VContainer.Unity;

namespace ForsakenGraves.Gameplay.Scope.CrossScene
{
    public class ApplicationScope : LifetimeScope
    {
        [FormerlySerializedAs("_connectionManager")] [SerializeField] ConnectionStateManager _connectionStateManager;
        
        protected override void Configure(IContainerBuilder builder)
        {
            base.Configure(builder);
            builder.RegisterComponent(_connectionStateManager);
            
            MessagePipeOptions options = RegisterMessagePipe(builder);


            // builder.RegisterComponentInHierarchy<CrossSceneView>();
            // builder.Register<CrossSceneService>(Lifetime.Singleton);
            
            builder.Register<AuthenticationServiceFacade>(Lifetime.Singleton);
            builder.Register<ProfileManager>(Lifetime.Singleton);
            
            builder.UseEntryPoints(Lifetime.Singleton, entryPoints =>
                                                       {
                                                           // entryPoints.Add<CrossScenePresenter>();
                                                           // entryPoints.Add<CrossSceneService>();
                                                           entryPoints.Add<MainMenuGameState>();
                                                       });
            
            builder.Register<LocalLobbyPlayer>(Lifetime.Singleton);
            builder.Register<LocalLobby>(Lifetime.Singleton);

            // builder.RegisterMessageBroker<string>(options);
            RegisterMessageBrokers(builder, options);

        }

        private static void RegisterMessageBrokers(IContainerBuilder builder, MessagePipeOptions options)
        {
            builder.RegisterMessageBroker<OnAuthenticationSuccessfulSignal>(options);
        }

        private MessagePipeOptions RegisterMessagePipe(IContainerBuilder builder)
        {
            MessagePipeOptions options = builder.RegisterMessagePipe();
            builder.RegisterBuildCallback(c => GlobalMessagePipe.SetProvider(c.AsServiceProvider()));
            return options;
        }
    }
}