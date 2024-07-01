using ForsakenGraves.Gameplay.Character;
using ForsakenGraves.Identifiers;
using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace ForsakenGraves.Gameplay.Weapons
{
    public class WeaponBuilderDirector : IInitializable
    {
        [Inject] private WeaponHolderSO _weaponHolderSO;
        private Camera _camera;

        public void Initialize()
        {
            _camera = Camera.main;
        }
        
        public Weapon BuildWeapon(WeaponID weaponID, ServerCharacter serverCharacter)
        {
            WeaponDataSO weaponDataSO = _weaponHolderSO.GetWeaponData(weaponID);
            WeaponType weaponType = weaponDataSO.WeaponType;

            return new Weapon.Builder()
                .WithOwner(serverCharacter)
                .WithWeaponData(weaponDataSO)
                .WithCamera(_camera)
                .Build();
        }
        
    }
}