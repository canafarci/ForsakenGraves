using UnityEngine;

namespace ForsakenGraves.Gameplay.Data
{
    [CreateAssetMenu(fileName = "Character Config", menuName = "ForsakenGraves/Data/Character Config", order = 0)]
    public class CharacterConfig : ScriptableObject
    {
        public float Health;
        public float Damage;
    }
}