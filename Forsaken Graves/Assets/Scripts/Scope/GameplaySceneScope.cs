using ForsakenGraves.Gameplay;
using ForsakenGraves.Gameplay.Cameras;
using ForsakenGraves.Gameplay.Data;
using ForsakenGraves.Gameplay.GameplayObjects;
using ForsakenGraves.Gameplay.Spawners;
using ForsakenGraves.Gameplay.Weapons;
using ForsakenGraves.GameState;
using ForsakenGraves.Infrastructure;
using ForsakenGraves.Infrastructure.Networking;
using ForsakenGraves.PreGame.Data;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.Serialization;
using VContainer;
using VContainer.Unity;

namespace ForsakenGraves.Scope
{
    public class GameplaySceneScope : LifetimeScope
    {
        [SerializeField] private PlayerAvatarsSO PlayerAvatarsSO; 
        [SerializeField] private NetcodeEvents NetcodeEvents;
        [SerializeField] private GameplaySceneConfig GameplaySceneConfig;
        [SerializeField] private WeaponHolderSO WeaponHolderSO;
        [SerializeField] private NetworkObjectPool NetworkObjectPool;

        protected override void Configure(IContainerBuilder builder)
        {
            builder.RegisterInstance(PlayerAvatarsSO);
            builder.RegisterInstance(GameplaySceneConfig);
            builder.RegisterInstance(NetcodeEvents);
            builder.RegisterInstance(WeaponHolderSO);
            builder.RegisterInstance(NetworkObjectPool);

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