
using ForsakenGraves.Gameplay.Character;
using ForsakenGraves.Identifiers;
using UnityEngine;

namespace ForsakenGraves.Gameplay.Weapons
{
    public abstract class Weapon
    {
        public Transform WeaponTransform { get; private set; }
        public Animator WeaponAnimator { get; private set; }
        public WeaponType WeaponType => _weaponDataSO.WeaponType;
        
        protected WeaponDataSO _weaponDataSO;
        protected Camera _mainCamera;
        protected ServerCharacter _ownerServerCharacter;
        
        protected float _lastFireTime;

        public GameObject WeaponPrefab => _weaponDataSO.Prefab;

        protected bool CanFire()
        {
            return Time.time - (_lastFireTime + _weaponDataSO.FireRate) > 0f;
        }

        public abstract void Fire();

        public void AttachToTransform(Transform parent)
        {
            WeaponTransform.parent = parent;
            WeaponTransform.localPosition = Vector3.zero;
            WeaponTransform.localRotation = Quaternion.identity;
        }

        public class Builder
        {
            private ServerCharacter _ownerServerCharacter;
            private WeaponDataSO _weaponDataSO;
            private Camera _mainCamera;
            private Transform _weaponTransform;
            private Animator _weaponAnimator;

            public Builder WithOwner(ServerCharacter serverCharacter)
            {
                _ownerServerCharacter = serverCharacter;
                return this;
            }
            
            public Builder WithWeaponData(WeaponDataSO weaponDataSO)
            {
                _weaponDataSO = weaponDataSO;
                return this;
            }
            
            public Builder WithCamera(Camera camera)
            {
                _mainCamera = camera;
                return this;
            }
            
            public Weapon Build()
            {
                _weaponTransform = GameObject.Instantiate(_weaponDataSO.Prefab).transform;
                _weaponAnimator = _weaponTransform.GetComponent<Animator>();

                WeaponType weaponType = _weaponDataSO.WeaponType;
                Weapon weapon = weaponType switch
                                {
                                    WeaponType.Bullet => new BulletWeapon(),
                                    WeaponType.Melee  => new MeleeWeapon(),
                                    _                 => new ProjectileWeapon()
                                };

                weapon._ownerServerCharacter = _ownerServerCharacter;
                weapon._weaponDataSO = _weaponDataSO;
                weapon._mainCamera = _mainCamera;
                weapon.WeaponTransform = _weaponTransform;
                weapon.WeaponAnimator = _weaponAnimator;

                return weapon;
            }
        }
    }
}