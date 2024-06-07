using ForsakenGraves.Gameplay;
using ForsakenGraves.Gameplay.Data;
using ForsakenGraves.Gameplay.Spawners;
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
        [SerializeField] private NetcodeEvents _netcodeEvents;
        [SerializeField] private GameplaySceneConfig _gameplaySceneConfig;

        protected override void Configure(IContainerBuilder builder)
        {
            builder.RegisterInstance(_playerAvatarsSO);
            builder.RegisterInstance(_gameplaySceneConfig);
            builder.RegisterInstance(_netcodeEvents);

            builder.RegisterComponentInHierarchy<ServerGameplaySceneState>().AsSelf();
            builder.RegisterComponentInHierarchy<ServerCharacterSpawnState>().AsSelf();
            builder.RegisterComponentInHierarchy<PlayerCharacterSpawner>().AsSelf();

            builder.RegisterEntryPoint<GameplaySettings>().AsSelf();
        }
    }
}