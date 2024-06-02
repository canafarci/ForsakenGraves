using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

namespace ForsakenGraves.PreGame.Data
{
    [CreateAssetMenu(fileName = "Player Avatars", menuName = "ForsakenGraves/Player Avatars", order = 0)]
    public class PlayerAvatarsSO : ScriptableObject
    {
        public List<GameObject> OtherPlayerAvatars;
        public List<GameObject> PlayableAvatars;
    }
}