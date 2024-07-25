using ForsakenGraves.Gameplay.Cameras;
using ForsakenGraves.Gameplay.Character;
using ForsakenGraves.Gameplay.Character.Player;
using ForsakenGraves.Gameplay.Character.Stats;
using ForsakenGraves.Gameplay.Data;
using ForsakenGraves.Gameplay.Inputs;
using ForsakenGraves.Infrastructure.Netcode;
using ForsakenGraves.PreGame.Data;
using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace ForsakenGraves.Scope
{
    public class PlayerCharacterScope : LifetimeScope
    {
        [SerializeField] private CapsuleCollider _capsuleCollider;
        [SerializeField] private LayerMask _targetMask;
        
        //data
        [SerializeField] private PlayerConfig _playerConfig;
        [SerializeField] private PlayerAvatarsSO _avatarsSO;
        [SerializeField] private CharacterConfig _characterConfig;
        [SerializeField] private PlayerAnimationData _playerAnimationData;
        
        protected override void Configure(IContainerBuilder builder)
        {
            builder.RegisterInstance(_capsuleCollider);
            builder.RegisterInstance(_targetMask);
            
            //data
            builder.RegisterInstance(_avatarsSO);
            builder.RegisterInstance(_playerConfig);
            builder.RegisterInstance(_characterConfig);
            builder.RegisterInstance(_playerAnimationData);

            builder.RegisterEntryPoint<InputPoller>().AsSelf();
            
            builder.RegisterComponentInHierarchy<PlayerCharacterGraphicsSpawner>().UnderTransform(transform).AsSelf();
            builder.RegisterComponentInHierarchy<ClientInventory>().UnderTransform(transform).AsSelf();
            builder.RegisterComponentInHierarchy<CharacterController>().UnderTransform(transform).AsSelf();
            builder.RegisterComponentInHierarchy<NetworkCharacterHealth>().UnderTransform(transform).AsSelf();
            builder.RegisterComponentInHierarchy<ClientCharacterPlayerDataObject>().UnderTransform(transform).AsSelf();
            builder.RegisterComponentInHierarchy<ServerCharacter>().UnderTransform(transform).AsSelf();
            builder.RegisterComponentInHierarchy<OwnerNetworkAnimator>().UnderTransform(transform).AsSelf();
            builder.RegisterComponentInHierarchy<AnticipatedPlayerController>().UnderTransform(transform).AsSelf();
            
            //camera
            builder.RegisterEntryPoint<CameraController>().AsSelf();
        }
    }
}