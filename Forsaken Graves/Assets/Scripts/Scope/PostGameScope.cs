using ForsakenGraves.GameState;
using ForsakenGraves.MainMenu;
using ForsakenGraves.PostGame;
using VContainer;
using VContainer.Unity;

namespace ForsakenGraves.Scope
{
    public class PostGameScope : LifetimeScope
    {
        protected override void Configure(IContainerBuilder builder)
        {
            builder.RegisterComponentInHierarchy<PostGamePanelView>().AsSelf();
            builder.RegisterComponentInHierarchy<ServerPostGameState>().AsSelf();
            
            builder.RegisterEntryPoint<PostGamePanelMediator>().AsSelf();
            builder.RegisterEntryPoint<PostGamePanelController>().AsSelf();
            builder.RegisterEntryPoint<UIMenuSceneSettings>();
        }
    }
}