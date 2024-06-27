
using ForsakenGraves.Gameplay.Character;
using UnityEngine;

namespace ForsakenGraves.Gameplay.Weapons
{
    public abstract class Weapon
    {
        protected readonly WeaponDataSO _weaponDataSO;
        protected readonly Camera _mainCamera;
        protected ServerCharacter _ownerServerCharacter;
        
        private float _lastFireTime;

        protected Weapon(WeaponDataSO weaponDataSO, Camera mainCamera, ServerCharacter ownerServerCharacter)
        {
            _weaponDataSO = weaponDataSO;
            _mainCamera = mainCamera;
            _ownerServerCharacter = ownerServerCharacter;
        }

        public bool CanFire()
        {
            return Time.time - (_lastFireTime + _weaponDataSO.FireRate) > 0f;
        }

        public abstract void Fire();
    }
}