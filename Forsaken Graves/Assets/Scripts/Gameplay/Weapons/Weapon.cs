
using System;
using Animancer;
using ForsakenGraves.Gameplay.Character;
using ForsakenGraves.Identifiers;
using ForsakenGraves.Visuals.Animations;
using Unity.Plastic.Newtonsoft.Json.Serialization;
using UnityEngine;

namespace ForsakenGraves.Gameplay.Weapons
{
    public abstract class Weapon
    {
        private Transform _weaponTransform;
        public AnimancerComponent WeaponAnimancer { get; private set; }
        
        protected WeaponDataSO _weaponDataSO;
        protected Camera _mainCamera;
        protected ServerCharacter _ownerServerCharacter;
        protected float _lastFireTime;
        protected WeaponState _weaponState = WeaponState.Idle;

        public static event Action<AnimationType> OnWeaponAnimationChanged; 

        public GameObject WeaponPrefab => WeaponDataSO.Prefab;
        public WeaponType WeaponType => WeaponDataSO.WeaponType;
        public WeaponDataSO WeaponDataSO => _weaponDataSO;

        protected bool CanFire()
        {
            return Time.time - (_lastFireTime + WeaponDataSO.FireRate) > 0f;
        }

        protected abstract void Fire();

        public void AttachToTransform(Transform parent)
        {
            _weaponTransform.parent = parent;
            _weaponTransform.localPosition = Vector3.zero;
            _weaponTransform.localRotation = Quaternion.identity;
        }
        
        public void StartFire()
        {
            if (_weaponState == WeaponState.Idle)
            {
                //WeaponAnimancer.SetBool(AnimationHashes.Shoot, true);
                _weaponState = WeaponState.Firing;
                OnWeaponAnimationChanged?.Invoke(AnimationType.Firing);
            }
            
            if (_weaponState == WeaponState.Reloading) return;

            Fire();
        }

        public void StopFire()
        {
            if (_weaponState == WeaponState.Firing)
            {
                //WeaponAnimator.SetBool(AnimationHashes.Shoot, false);
                _weaponState = WeaponState.Idle;
                OnWeaponAnimationChanged?.Invoke(AnimationType.Idle);
            }
        }
        
        protected enum WeaponState
        {
            Firing,
            Idle,
            Reloading
        }

        public class Builder
        {
            private ServerCharacter _ownerServerCharacter;
            private WeaponDataSO _weaponDataSO;
            private Camera _mainCamera;
            private Transform _weaponTransform;
            private AnimancerComponent _weaponAnimancer;

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
                _weaponAnimancer = _weaponTransform.GetComponent<AnimancerComponent>();

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
                weapon._weaponTransform = _weaponTransform;
                weapon.WeaponAnimancer = _weaponAnimancer;

                return weapon;
            }
        }
    }
}