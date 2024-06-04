using ForsakenGraves.Gameplay.UI;
using ForsakenGraves.MainMenu;
using ForsakenGraves.PreGame.Data;
using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace ForsakenGraves.Scope
{
    public class MainMenuScope : LifetimeScope
    {
        protected override void Configure(IContainerBuilder builder)
        {
            builder.RegisterComponentInHierarchy<LobbyCreationView>();
            builder.RegisterComponentInHierarchy<LobbyJoiningView>();
            
            builder.RegisterEntryPoint<LobbyCreationMediator>();
            builder.RegisterEntryPoint<LobbyJoiningMediator>();
            builder.RegisterEntryPoint<MainMenuSettings>();
        }
    }
}

