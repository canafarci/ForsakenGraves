using ForsakenGraves.Gameplay;
using ForsakenGraves.Gameplay.Cameras;
using ForsakenGraves.Gameplay.Data;
using ForsakenGraves.Gameplay.Spawners;
using ForsakenGraves.Gameplay.Weapons;
using ForsakenGraves.GameState;
using ForsakenGraves.Infrastructure;
using ForsakenGraves.Infrastructure.Networking;
using ForsakenGraves.PreGame.Data;
using Unity.Cinemachine;
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
        [SerializeField] private WeaponHolderSO _weaponHolderSO;

        protected override void Configure(IContainerBuilder builder)
        {
            builder.RegisterInstance(_playerAvatarsSO);
            builder.RegisterInstance(_gameplaySceneConfig);
            builder.RegisterInstance(_netcodeEvents);
            builder.RegisterInstance(_weaponHolderSO);

            builder.RegisterComponentInHierarchy<ServerGameplaySceneState>().AsSelf();
            builder.RegisterComponentInHierarchy<ServerCharacterSpawnState>().AsSelf();
            builder.RegisterComponentInHierarchy<PlayerCharacterSpawner>().AsSelf();

            builder.RegisterEntryPoint<GameplaySettings>().AsSelf();
            
            //weapon
            builder.RegisterEntryPoint<WeaponBuilderDirector>().AsSelf();
            //camera
            builder.RegisterComponentInHierarchy<CinemachineBrain>().AsSelf();
            builder.RegisterEntryPoint<CameraTicker>().AsSelf();
            //network
            builder.RegisterEntryPoint<NetworkTicker>().AsSelf();
        }
    }
}