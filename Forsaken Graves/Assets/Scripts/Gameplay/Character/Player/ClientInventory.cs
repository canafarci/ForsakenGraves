using System.Collections.Generic;
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
        [Inject] private KRigComponent _rigComponent;
        
        private Transform _weaponBone;
        private List<Weapon> _playerWeapons = new();
        public Weapon ActiveWeapon { get; private set; }

        public override void OnNetworkSpawn()
        {
            if (!IsOwner) return;
            
            _weaponBone = _rigComponent.GetRigTransform(_playerAnimationData.WeaponBone);
            //FOR DEVELOPMENT
            PickUpWeapon(WeaponID.AssaultRifle);
        }

        public void PickUpWeapon(WeaponID weaponID)
        {
            Weapon weapon = _weaponFactory.CreateWeapon(weaponID, _serverCharacter);
            
            _playerWeapons.Add(weapon);
            InstantiateWeaponGraphics(weapon);
            EquipWeapon(weapon);
        }

        public void EquipWeapon(Weapon weapon)
        {
            ActiveWeapon = weapon;
        }

        private void InstantiateWeaponGraphics(Weapon weapon)
        {
            Transform weaponTransform = Instantiate(weapon.WeaponPrefab, transform.position, Quaternion.identity).transform;
            
            weaponTransform.parent = _weaponBone;
            weaponTransform.localPosition = Vector3.zero;
            weaponTransform.localRotation = Quaternion.identity;
            
        }
    }
}