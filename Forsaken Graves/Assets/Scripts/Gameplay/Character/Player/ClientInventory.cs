#pragma warning disable CS4014

using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using ForsakenGraves.Gameplay.Data;
using ForsakenGraves.Gameplay.Weapons;
using ForsakenGraves.Identifiers;
using ForsakenGraves.Visuals.Animations;
using Unity.Netcode;
using UnityEngine;
using VContainer;

namespace ForsakenGraves.Gameplay.Character.Player
{
    public class ClientInventory : NetworkBehaviour
    {
        [Inject] private WeaponBuilderDirector _weaponBuilderDirector;
        [Inject] private ServerCharacter _serverCharacter;
        [Inject] private PlayerAnimationData _playerAnimationData;
        [Inject] private PlayerCharacterGraphicsSpawner _graphicsSpawner;

        private HandsFacade _handsFacade;
        
        private readonly List<Weapon> _playerWeapons = new();
        public Weapon ActiveWeapon { get; private set; }

        private void Awake()
        {
            _graphicsSpawner.OnAvatarSpawned += AvatarSpawnedHandler;
        }

        private void AvatarSpawnedHandler()
        {
            _handsFacade = GetComponentInChildren<HandsFacade>();
        }

        public override void OnNetworkSpawn()
        {
            if (!IsOwner) return;
            
            //FOR DEVELOPMENT
            PickUpWeapon(WeaponID.TommyGun);
        }

        private void PickUpWeapon(WeaponID weaponID)
        {
            Weapon weapon = _weaponBuilderDirector.BuildWeapon(weaponID, _serverCharacter);
            
            _playerWeapons.Add(weapon);
            InstantiateWeaponGraphics(weapon);
            EquipWeapon(weapon);
        }

        private void EquipWeapon(Weapon weapon)
        {
            ActiveWeapon = weapon;
        }

        private async void InstantiateWeaponGraphics(Weapon weapon)
        {
            await UniTask.WaitWhile(() => _handsFacade == null);
            _handsFacade.InitializeWeapon(weapon);
        }

        public override void OnNetworkDespawn()
        {
            _graphicsSpawner.OnAvatarSpawned -= AvatarSpawnedHandler;
        }
    }
}