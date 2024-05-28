using ForsakenGraves.GameState;
using ForsakenGraves.Infrastructure;
using ForsakenGraves.PreGame;
using ForsakenGraves.PreGame.AvatarSelect;
using ForsakenGraves.PreGame.Data;
using ForsakenGraves.PreGame.UI.AvatarSelect;
using ForsakenGraves.PreGame.UI.ReadyPanel;
using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace ForsakenGraves.Scope
{
    public class PreGameScope : LifetimeScope
    {
        [SerializeField] private PlayerAvatarsSO _playerAvatarsSO; 
        [SerializeField] private ServerPreGameState _serverPreGameState; 
        [SerializeField] private NetcodeEvents _netcodeEvents; 
        [SerializeField] private PreGameNetwork _preGameNetwork; 

        protected override void Configure(IContainerBuilder builder)
        {
            builder.RegisterInstance(_playerAvatarsSO);
            builder.RegisterInstance(_serverPreGameState);
            builder.RegisterInstance(_netcodeEvents);
            builder.RegisterInstance(_preGameNetwork);
            
            //ready panel
            builder.RegisterComponentInHierarchy<ReadyPanelView>();
            builder.RegisterEntryPoint<ReadyPanelMediator>().AsSelf();
            builder.RegisterEntryPoint<ReadyPanelController>().AsSelf();
            
            //avatar select
            builder.RegisterComponentInHierarchy<AvatarDisplayCompositeView>();
            builder.RegisterComponentInHierarchy<AvatarSelectView>();
            builder.Register<AvatarSelectModel>(Lifetime.Singleton);
            builder.RegisterEntryPoint<AvatarSelectController>().AsSelf();
            builder.RegisterEntryPoint<AvatarSelectMediator>().AsSelf();
            builder.RegisterEntryPoint<AvatarDisplayService>().AsSelf();
            
        }
    }
}