using ForsakenGraves.Gameplay.Character;
using ForsakenGraves.Gameplay.Character.Player;
using ForsakenGraves.Gameplay.Character.Stats;
using ForsakenGraves.Gameplay.Data;
using ForsakenGraves.Gameplay.Inputs;
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
        [SerializeField] private CharacterController _characterController;
        [SerializeField] private CapsuleCollider _capsuleCollider;
        [SerializeField] private ClientCharacterPlayerDataObject _clientCharacterPlayerDataObject;
        [SerializeField ] private LayerMask _targetMask;
        [SerializeField ] private NetworkCharacterHealth _characterHealth;
        
        //data
        [SerializeField] private PlayerConfig _playerConfig;
        [SerializeField] private PlayerAvatarsSO _avatarsSO;
        [SerializeField ] private CharacterConfig _characterConfig;
        
        protected override void Configure(IContainerBuilder builder)
        {
            builder.RegisterInstance(_serverCharacter);
            builder.RegisterInstance(_playerConfig);
            builder.RegisterInstance(_characterController);
            builder.RegisterInstance(_capsuleCollider);
            builder.RegisterInstance(_avatarsSO);
            builder.RegisterInstance(_clientCharacterPlayerDataObject);
            builder.RegisterInstance(_targetMask);
            builder.RegisterInstance(_characterConfig);
            builder.RegisterInstance(_characterHealth);

            builder.RegisterEntryPoint<InputPoller>().AsSelf();
        }
    }
}