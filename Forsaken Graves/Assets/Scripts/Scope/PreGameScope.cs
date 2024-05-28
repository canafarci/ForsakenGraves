using ForsakenGraves.GameState;
using ForsakenGraves.PreGame;
using ForsakenGraves.PreGame.UI.ReadyPanel;
using VContainer;
using VContainer.Unity;

namespace ForsakenGraves.Scope
{
    public class PreGameScope : LifetimeScope
    {
        protected override void Configure(IContainerBuilder builder)
        {
            base.Configure(builder);

            builder.RegisterComponentInHierarchy<ServerPreGameState>();
            builder.RegisterComponentInHierarchy<ClientNetworkedPreGameLogic>();
            
            builder.RegisterComponentInHierarchy<ReadyPanelView>();
            builder.RegisterEntryPoint<ReadyPanelMediator>().AsSelf();
            builder.RegisterEntryPoint<ReadyPanelController>().AsSelf();
        }
    }
}