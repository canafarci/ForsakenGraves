using System.Collections.Generic;
using ForsakenGraves.Identifiers;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Assertions;

namespace ForsakenGraves.Gameplay.Weapons
{
    [CreateAssetMenu(fileName = "Weapon Holder SO", menuName = "ForsakenGraves/Weapons/Weapon Holder SO", order = 0)]
    public class WeaponHolderSO : SerializedScriptableObject
    {
        [SerializeField] private Dictionary<WeaponID, WeaponDataSO> _weaponDataLookup;

        public WeaponDataSO GetWeaponData(WeaponID weaponID)
        {
            WeaponDataSO weaponDataSO = _weaponDataLookup[weaponID];
            Assert.IsNotNull(weaponDataSO, $"Weapon data for weapon with ID {weaponID} has not been found!");
            
            return weaponDataSO;
        }
    }
}