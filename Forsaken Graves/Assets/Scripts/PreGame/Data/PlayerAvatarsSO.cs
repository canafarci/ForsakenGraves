using System.Collections.Generic;
using UnityEngine;

namespace ForsakenGraves.PreGame.Data
{
    [CreateAssetMenu(fileName = "Player Avatars", menuName = "ForsakenGraves/Player Avatars", order = 0)]
    public class PlayerAvatarsSO : ScriptableObject
    {
        public List<GameObject> PlayerAvatars;
    }
}