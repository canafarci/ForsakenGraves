using System;
using Animancer;
using Cysharp.Threading.Tasks;
using ForsakenGraves.Gameplay.Character.Player;
using ForsakenGraves.Gameplay.Data;
using ForsakenGraves.Gameplay.Weapons;
using UnityEngine;
using UnityEngine.Serialization;
using VContainer;

namespace ForsakenGraves.Visuals.Animations
{
    public class HandsFacade : MonoBehaviour
    {
        [Inject] private PlayerCharacterGraphicsSpawner _graphicsSpawner;
        
        [SerializeField] private AnimancerComponent _handsAnimancer;
        [SerializeField] private FollowHands _followHands;
        [SerializeField] private HandsSpawnTransform _handsSpawnTransform;

        private Weapon _weapon;
        
        public AnimancerComponent HandsAnimancer => _handsAnimancer;
        public FollowHands FollowHands => _followHands;
        public LinearMixerTransitionAsset.UnShared LinearMixer => _weapon.WeaponDataSO.LinearMixerTransitionAsset;

        private HandsInitializationState _handsInitializationState = HandsInitializationState.NoneInitialized;
        
        public void InitializeWeapon(Weapon weapon)
        {
            _weapon = weapon;
            
            weapon.AttachToTransform(_handsSpawnTransform.transform);
            SetUpAnimations(weapon);

            UpdateInitializationState(HandsInitializationState.WeaponInitialized);
        }

        private void SetUpAnimations(Weapon weapon)
        {
            _handsAnimancer = weapon.WeaponAnimancer;
            _handsAnimancer.Play(weapon.WeaponDataSO.LinearMixerTransitionAsset);
        }

        public void InitializeHandsFollow(Transform targetReferenceHandsFollowTransform, 
                                          PlayerConfig playerConfig,
                                          AnticipatedPlayerController anticipatedPlayerController)
        {
            _followHands ??= GetComponentInChildren<FollowHands>();

            _followHands.Initialize(targetReferenceHandsFollowTransform,
                                    playerConfig,
                                    anticipatedPlayerController);
            
            UpdateInitializationState(HandsInitializationState.FollowInitialized);

        }

        private void UpdateInitializationState(HandsInitializationState handsInitializationState)
        {
            if (_handsInitializationState.HasFlag(handsInitializationState)) return;
            
            _handsInitializationState |= handsInitializationState;
            CheckIfAllModulesInitialized();
        }

        private void CheckIfAllModulesInitialized()
        {
            if (_handsInitializationState.HasFlag(HandsInitializationState.FollowInitialized) &&
                _handsInitializationState.HasFlag(HandsInitializationState.AnimatorInitialized ) &&
                _handsInitializationState.HasFlag(HandsInitializationState.WeaponInitialized))
            {
                HandleInitializationFinished();
            }
        }

        private void HandleInitializationFinished()
        {
            _followHands.transform.SetParent(null);
        }

        [Flags]
        private enum HandsInitializationState
        {
            NoneInitialized = 0,
            FollowInitialized = 1,
            AnimatorInitialized = 2,
            WeaponInitialized = 3,
            InitializationFinished = 99
        }
    }
}