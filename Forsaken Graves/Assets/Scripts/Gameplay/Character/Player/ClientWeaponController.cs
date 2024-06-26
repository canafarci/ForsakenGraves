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
        [Inject] private CharacterConfig _characterConfig;
        // Layer mask to filter which objects should be considered for the raycast
        [Inject ] private LayerMask _targetMask;
        [Inject] private ServerCharacter _serverCharacter;

        readonly int halfScreenWidth = Screen.width / 2;
        readonly int halfScreenHeight = Screen.height / 2;
        
        // Maximum distance for the raycast
        private float _maxDistance = Mathf.Infinity;
        private Camera _camera;

        private WeaponSO _weapon;
        
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
            
            // Cast a ray from the center of the screen
            Ray ray = _camera.ScreenPointToRay(new Vector3(halfScreenWidth, halfScreenHeight, 0));

            // Perform the raycast
            if (Physics.Raycast(ray, out RaycastHit hitInfo, _maxDistance, _targetMask))
            {
                // Check if the hit object implements the ITargetable interface
                if (hitInfo.collider.TryGetComponent(out ITargetable target))
                {
                    float damage = _weapon == null ? _characterConfig.Damage : _weapon.Damage;
                    _serverCharacter.DamageTargetServerRpc(hitInfo.transform.gameObject, damage);
                }
            }
        }

        private bool CantShoot() => !_inputPoller.GetShootingInput() || _camera == null || !IsSpawned;
    }
}