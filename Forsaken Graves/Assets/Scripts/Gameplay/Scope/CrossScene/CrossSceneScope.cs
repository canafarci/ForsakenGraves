using ForsakenGraves.Connection;
using ForsakenGraves.Gameplay.GameState;
using ForsakenGraves.UnityService.Auth;
using ForsakenGraves.UnityService.Lobbies;
using MessagePipe;
using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace ForsakenGraves.Gameplay.Scope.CrossScene
{
    public class CrossSceneScope : LifetimeScope
    {
        [SerializeField] ConnectionManager _connectionManager;
        
        protected override void Configure(IContainerBuilder builder)
        {
            base.Configure(builder);
            builder.RegisterComponent(_connectionManager);
            
            
            MessagePipeOptions options = RegisterMessagePipe(builder);


            // builder.RegisterComponentInHierarchy<CrossSceneView>();
            // builder.Register<CrossSceneService>(Lifetime.Singleton);
            
            builder.Register<AuthenticationServiceFacade>(Lifetime.Singleton);
            
            builder.UseEntryPoints(Lifetime.Singleton, entryPoints =>
                                                       {
                                                           // entryPoints.Add<CrossScenePresenter>();
                                                           // entryPoints.Add<CrossSceneService>();
                                                           entryPoints.Add<MainMenuGameState>();
                                                       });

            
            
            builder.Register<LocalLobbyPlayer>(Lifetime.Singleton);

            // builder.RegisterMessageBroker<string>(options);
            // builder.RegisterMessageBroker<int>(options);

        }

        private MessagePipeOptions RegisterMessagePipe(IContainerBuilder builder)
        {
            MessagePipeOptions options = builder.RegisterMessagePipe();
            builder.RegisterBuildCallback(c => GlobalMessagePipe.SetProvider(c.AsServiceProvider()));
            return options;
        }
    }
}