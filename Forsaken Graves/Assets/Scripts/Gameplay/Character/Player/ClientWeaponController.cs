using System;
using ForsakenGraves.Gameplay.Character.Stats;
using ForsakenGraves.Gameplay.Data;
using ForsakenGraves.Gameplay.Inputs;
using ForsakenGraves.Gameplay.Weapons;
using Unity.Netcode;
using UnityEngine;
using VContainer;

namespace ForsakenGraves.Gameplay.Character.Player
{
    public class ClientWeaponController : NetworkBehaviour
    {
        [Inject] private InputPoller _inputPoller;
        [Inject] private ClientInventory _clientInventory;
        
        private Camera _camera;
        
        public override void OnNetworkSpawn()
        {
            if (!IsOwner)
            {
                enabled = false;
                return;
            }
            
            _camera = Camera.main;
        }

        private void Update()
        {
            // Check if the middle of the screen is being clicked
            if (CantShoot()) return;
            
            if (CanFire())
                _clientInventory.ActiveWeapon.Fire();
        }

        private bool CantShoot() => _camera == null || _clientInventory.ActiveWeapon == null || !IsSpawned;
        private bool CanFire() => _inputPoller.GetShootingInput() && _clientInventory.ActiveWeapon.CanFire();
    }
}