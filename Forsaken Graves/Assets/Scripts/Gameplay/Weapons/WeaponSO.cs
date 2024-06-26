using UnityEngine;

namespace ForsakenGraves.Gameplay.Weapons
{
    [CreateAssetMenu(fileName = "WeaponSO", menuName = "ForsakenGraves", order = 0)]
    public class WeaponSO : ScriptableObject
    {
        public string Name;
        public float Damage;
        public GameObject Prefab;
    }
}