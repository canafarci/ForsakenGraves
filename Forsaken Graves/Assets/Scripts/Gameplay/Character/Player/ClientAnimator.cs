using System;
using Animancer;
using ForsakenGraves.Gameplay.Data;
using ForsakenGraves.Gameplay.Weapons;
using ForsakenGraves.Identifiers;
using ForsakenGraves.Infrastructure.Netcode;
using ForsakenGraves.Infrastructure.Networking;
using ForsakenGraves.Visuals.Animations;
using Unity.Netcode;
using UnityEngine;
using VContainer;

namespace ForsakenGraves.Gameplay.Character.Player
{
    public class ClientAnimator : NetworkBehaviour
    {
        [Inject] private OwnerNetworkAnimator _ownerNetworkAnimator;
        [Inject] private ClientInventory _clientInventory;
        [Inject] private PlayerCharacterGraphicsSpawner _graphicsSpawner;
        [Inject] private PlayerConfig _playerConfig;
        [Inject] private AnticipatedPlayerController _anticipatedPlayerController;
        
        private Vector3 _lastPosition;
        private Vector3 _currentPosition;
        private HandsFacade _handsFacade;
        private float _lastMovementSpeed;

        private void Awake()
        {
            _graphicsSpawner.OnAvatarSpawned += AvatarSpawnedHandler;
            _anticipatedPlayerController.OnTransformUpdated += PositionUpdatedHandler;
        }

        public override void OnNetworkSpawn()
        {
            if (!IsOwner)
            {
                enabled = false;
                return;
            }

            Weapon.OnWeaponAnimationChanged += WeaponAnimationChangedHandler;
        }

        private void WeaponAnimationChangedHandler(AnimationType animationType)
        {
            Animator animator = _ownerNetworkAnimator.Animator;
            switch (animationType)
            {
                case AnimationType.Firing:
                    AnimancerState state =  _handsFacade.HandsAnimancer.Play(_clientInventory.ActiveWeapon.WeaponDataSO.FireAnimationClip);
                    state.Events.Add(0.1f, PlayWeaponFX);
                    animator.SetBool(AnimationHashes.Shoot, true);
                    break;
                
                case AnimationType.Idle:
                    _handsFacade.HandsAnimancer.Play(_clientInventory.ActiveWeapon.WeaponDataSO.LinearMixerTransitionAsset);
                    animator.SetBool(AnimationHashes.Shoot, false);
                    break;
            }
        }

        private void PlayWeaponFX()
        {
            Weapon activeWeapon = _clientInventory.ActiveWeapon;
            AudioClip clip = activeWeapon.WeaponDataSO.GetRandomFireSound();
            activeWeapon.FireParticleSystem.Play();
            activeWeapon.FireAudioSource.PlayOneShot(clip);
        }

        private  void AvatarSpawnedHandler()
        {
            _ownerNetworkAnimator.Animator.Rebind();

            if (IsOwner)
            {
                _handsFacade = GetComponentInChildren<HandsFacade>();
            }
        }
        
        private void PositionUpdatedHandler()
        {
            if (!IsOwner || _handsFacade == null ||_handsFacade.HandsAnimancer == null) return;
            
            _currentPosition = transform.position;
            
            float movementSpeed = Vector3.SqrMagnitude(_lastPosition - _currentPosition) /
                                  (float)Math.Pow(_playerConfig.MovementSpeed * NetworkTicker.TickRate, 2);
            
            if (movementSpeed > 1) movementSpeed = 1;
            movementSpeed = Mathf.Lerp(_lastMovementSpeed, movementSpeed, NetworkTicker.TickRate * 5f);

            _handsFacade.LinearMixer.State.Parameter = movementSpeed;
            _ownerNetworkAnimator.Animator.SetFloat(AnimationHashes.MovementSpeed, 
                                                    movementSpeed);
            
            _lastPosition = _currentPosition;
            _lastMovementSpeed = movementSpeed;
        }

        public override void OnNetworkDespawn()
        {
            _graphicsSpawner.OnAvatarSpawned -= AvatarSpawnedHandler;
            _anticipatedPlayerController.OnTransformUpdated -= PositionUpdatedHandler;
            
            if (IsOwner)
            {
                Weapon.OnWeaponAnimationChanged -= WeaponAnimationChangedHandler;
            }
        }
    }
}