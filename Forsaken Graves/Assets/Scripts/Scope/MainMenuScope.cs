using ForsakenGraves.Gameplay.UI;
using VContainer;
using VContainer.Unity;

namespace ForsakenGraves.Scope
{
    public class MainMenuScope : LifetimeScope
    {
        protected override void Configure(IContainerBuilder builder)
        {
            base.Configure(builder);
            
            builder.RegisterComponentInHierarchy<LobbyCreationView>();
            builder.RegisterComponentInHierarchy<LobbyJoiningView>();
            
            builder.RegisterEntryPoint<LobbyCreationMediator>();
            builder.RegisterEntryPoint<LobbyJoiningMediator>();
        }
    }
}

