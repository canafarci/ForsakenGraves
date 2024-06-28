using ForsakenGraves.Gameplay.Character;
using ForsakenGraves.Identifiers;
using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace ForsakenGraves.Gameplay.Weapons
{
    public class WeaponFactory : IInitializable
    {
        [Inject] private WeaponHolderSO _weaponHolderSO;
        private Camera _camera;

        public void Initialize()
        {
            _camera = Camera.main;
        }
        
        public Weapon CreateWeapon(WeaponID weaponID, ServerCharacter serverCharacter)
        {
            WeaponDataSO weaponDataSO = _weaponHolderSO.GetWeaponData(weaponID);
            WeaponType weaponType = weaponDataSO.WeaponType;

            return weaponType switch
                   {
                       WeaponType.Bullet => CreateBulletWeapon(weaponDataSO, serverCharacter),
                       WeaponType.Melee  => CreateMeleeWeapon(weaponDataSO, serverCharacter),
                       _                 => CreateProjectileWeapon(weaponDataSO, serverCharacter)
                   };
        }

        private Weapon CreateProjectileWeapon(WeaponDataSO weaponDataSO, ServerCharacter serverCharacter)
        {
            throw new System.NotImplementedException();
        }

        private Weapon CreateMeleeWeapon(WeaponDataSO weaponDataSO, ServerCharacter serverCharacter)
        {
            throw new System.NotImplementedException();
        }

        private Weapon CreateBulletWeapon(WeaponDataSO weaponDataSO, ServerCharacter serverCharacter)
        {
            return new BulletWeapon(weaponDataSO, _camera, serverCharacter);
        }
    }
}