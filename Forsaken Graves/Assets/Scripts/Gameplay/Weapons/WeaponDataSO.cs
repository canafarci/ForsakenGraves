using Animancer;
using ForsakenGraves.Identifiers;
using UnityEngine;

namespace ForsakenGraves.Gameplay.Weapons
{
    [CreateAssetMenu(fileName = "Weapon Data SO", menuName = "ForsakenGraves/Weapons/Weapon Data SO", order = 0)]
    public class WeaponDataSO : ScriptableObject
    {
        public string Name;
        public WeaponType WeaponType;
        
        public float Damage;
        [Min(0f)] public float FireRate;
        public LayerMask TargetLayerMask;
        
        public GameObject Prefab;
        public LinearMixerTransitionAsset.UnShared LinearMixerTransitionAsset;
        public AnimationClip FireAnimationClip;
        
        [SerializeField] private AudioClip[] FireSounds;

        public AudioClip GetRandomFireSound()
        {
            int random = (int)Random.Range(0, FireSounds.Length);
            AudioClip clip = FireSounds[random];
            return clip;
        }
    }
}