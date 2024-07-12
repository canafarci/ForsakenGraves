using Sirenix.OdinInspector;
using UnityEngine;

namespace ForsakenGraves.Gameplay.Data
{
    [CreateAssetMenu(fileName = "Animation Config", menuName = "ForsakenGraves/Data/Animation Config", order = 0)]
    public class AnimationConfig : SerializedScriptableObject
    {
        public float HandsLerpSpeed;
        public float HandsSlerpSpeed;
    }
}