using UnityEngine;

namespace ForsakenGraves.Gameplay.Weapons
{
    public class WeaponProjectileSpawnPoint : MonoBehaviour
    {
        [SerializeField] private Transform ProjectileSpawnPoint;

        public Transform projectileSpawnPoint => ProjectileSpawnPoint;
    }
}