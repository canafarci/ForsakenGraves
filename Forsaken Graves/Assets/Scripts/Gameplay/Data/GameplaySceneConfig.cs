using UnityEngine;

namespace ForsakenGraves.Gameplay.Data
{
    [CreateAssetMenu(fileName = "Gameplay Scene Config", menuName = "ForsakenGraves/Data/Gameplay Scene Config", order = 0)]
    public class GameplaySceneConfig : ScriptableObject
    {
        public int EnemyCountToSpawn;
    }
}