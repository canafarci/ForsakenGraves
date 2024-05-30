using UnityEngine;

namespace ForsakenGraves.Gameplay.Character
{
    //data object for passing data from pregame scene
    public class ClientCharacterPlayerDataObject : MonoBehaviour
    {
        public int AvatarIndex { get; set; }
        public string DisplayName { get; set; }
    }
}