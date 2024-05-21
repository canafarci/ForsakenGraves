using ForsakenGraves.Gameplay.UI;
using ForsakenGraves.UnityService.Auth;
using ForsakenGraves.UnityService.Lobbies;
using VContainer;
using VContainer.Unity;

namespace ForsakenGraves.Gameplay.Scope
{
    public class MainMenuScope : LifetimeScope
    {
        protected override void Configure(IContainerBuilder builder)
        {
            builder.RegisterComponentInHierarchy<LobbyCreationView>();
            
            builder.UseEntryPoints(Lifetime.Singleton, entryPoints =>
                                                       {
                                                           entryPoints.Add<LobbyServiceFacade>();
                                                           entryPoints.Add<LobbyCreationMediator>();
                                                       });
        }
    }
}

