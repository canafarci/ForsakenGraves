using ForsakenGraves.Identifiers;
using UnityEngine;

namespace ForsakenGraves.Gameplay.Weapons
{
    [CreateAssetMenu(fileName = "WeaponDataSO", menuName = "ForsakenGraves/Weapons", order = 0)]
    public class WeaponDataSO : ScriptableObject
    {
        public string Name;
        public WeaponType WeaponType;
        
        public float Damage;
        [Min(0f)] public float FireRate;
        public LayerMask TargetLayerMask;
        
        public GameObject Prefab;
    }
}