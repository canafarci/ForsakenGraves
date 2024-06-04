using ForsakenGraves.Gameplay;
using ForsakenGraves.GameState;
using ForsakenGraves.Infrastructure;
using ForsakenGraves.PreGame.Data;
using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace ForsakenGraves.Scope
{
    public class GameplaySceneScope : LifetimeScope
    {
        [SerializeField] private PlayerAvatarsSO _playerAvatarsSO; 
        [SerializeField] private ServerGameplaySceneState _serverGameplaySceneState; 
        [SerializeField] private NetcodeEvents _netcodeEvents;

        protected override void Configure(IContainerBuilder builder)
        {
            builder.RegisterInstance(_playerAvatarsSO);
            builder.RegisterInstance(_serverGameplaySceneState);
            builder.RegisterInstance(_netcodeEvents);

            builder.RegisterEntryPoint<GameplaySettings>().AsSelf();
        }
    }
}