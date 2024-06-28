#pragma warning disable CS4014

using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using ForsakenGraves.Gameplay.Data;
using ForsakenGraves.Gameplay.Weapons;
using ForsakenGraves.Identifiers;
using KINEMATION.KAnimationCore.Runtime.Rig;
using Unity.Netcode;
using UnityEngine;
using VContainer;

namespace ForsakenGraves.Gameplay.Character.Player
{
    public class ClientInventory : NetworkBehaviour
    {
        [Inject] private WeaponFactory _weaponFactory;
        [Inject] private ServerCharacter _serverCharacter;
        [Inject] private PlayerAnimationData _playerAnimationData;
        [Inject] private PlayerCharacterGraphicsSpawner _graphicsSpawner;
        
        private Transform _weaponBone;
        
        private List<Weapon> _playerWeapons = new();
        public Weapon ActiveWeapon { get; private set; }

        private void Awake()
        {
            _graphicsSpawner.OnAvatarSpawned += AvatarSpawnedHandler;
        }

        private void AvatarSpawnedHandler()
        {
            _weaponBone = GetComponentInChildren<KRigComponent>().GetRigTransform(_playerAnimationData.WeaponBone);
        }

        public override void OnNetworkSpawn()
        {
            if (!IsOwner) return;
            
            //FOR DEVELOPMENT
            PickUpWeapon(WeaponID.AssaultRifle);
        }

        private void PickUpWeapon(WeaponID weaponID)
        {
            Weapon weapon = _weaponFactory.CreateWeapon(weaponID, _serverCharacter);
            
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
            await UniTask.WaitWhile(() => _weaponBone == null);
            
            Transform weaponTransform = Instantiate(weapon.WeaponPrefab, transform.position, Quaternion.identity).transform;
            
            weaponTransform.parent = _weaponBone;
            weaponTransform.localPosition = Vector3.zero;
            weaponTransform.localRotation = Quaternion.identity;
        }

        public override void OnNetworkDespawn()
        {
            _graphicsSpawner.OnAvatarSpawned -= AvatarSpawnedHandler;
        }
    }
}