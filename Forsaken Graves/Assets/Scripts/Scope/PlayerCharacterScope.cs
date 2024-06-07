using ForsakenGraves.Gameplay.Character;
using ForsakenGraves.Gameplay.Character.Player;
using ForsakenGraves.Gameplay.Data;
using ForsakenGraves.PreGame.Data;
using Unity.Netcode.Components;
using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace ForsakenGraves.Scope
{
    public class PlayerCharacterScope : LifetimeScope
    {
        [SerializeField] private ServerCharacter _serverCharacter;
        [SerializeField] private PlayerConfig _playerConfig;
        [SerializeField] private AnticipatedNetworkTransform _anticipatedNetworkTransform;
        [SerializeField] private CharacterController _characterController;
        [SerializeField] private CapsuleCollider _capsuleCollider;
        [SerializeField] private PlayerAvatarsSO _avatarsSO;
        [SerializeField] private ClientCharacterPlayerDataObject _clientCharacterPlayerDataObject;
        
        protected override void Configure(IContainerBuilder builder)
        {
            builder.RegisterInstance(_serverCharacter);
            builder.RegisterInstance(_playerConfig);
            builder.RegisterInstance(_anticipatedNetworkTransform);
            builder.RegisterInstance(_characterController);
            builder.RegisterInstance(_capsuleCollider);
            builder.RegisterInstance(_avatarsSO);
            builder.RegisterInstance(_clientCharacterPlayerDataObject);

            builder.Register<InputPoller>(Lifetime.Singleton);
        }
    }
}