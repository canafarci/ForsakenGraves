using ForsakenGraves.Gameplay.Character;
using ForsakenGraves.Gameplay.Character.Stats;
using ForsakenGraves.Visuals.Animations;
using UnityEngine;

namespace ForsakenGraves.Gameplay.Weapons
{
    public class BulletWeapon : Weapon
    {
        readonly int halfScreenWidth = Screen.width / 2;
        readonly int halfScreenHeight = Screen.height / 2;
        
        // Maximum distance for the raycast
        private float _maxDistance = Mathf.Infinity;
        
        // Layer mask to filter which objects should be considered for the raycast
        public override void Fire()
        {
            if (!CanFire()) return;
            
            _lastFireTime = Time.time;
            
            WeaponAnimator.SetTrigger(AnimationHashes.Shoot);
            Ray ray = _mainCamera.ScreenPointToRay(new Vector3(halfScreenWidth, halfScreenHeight, 0));

            // Perform the raycast
            if (Physics.Raycast(ray, out RaycastHit hitInfo, _maxDistance, _weaponDataSO.TargetLayerMask))
            {
                // Check if the hit object implements the ITargetable interface
                if (hitInfo.collider.TryGetComponent(out ITargetable target))
                {
                    _ownerServerCharacter.DamageTargetServerRpc(hitInfo.transform.gameObject, _weaponDataSO.Damage);
                }
            }            
        }
    }
}