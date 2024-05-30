using Unity.Collections;
using Unity.Netcode;
using UnityEngine;

namespace ForsakenGraves.Gameplay.Character
{
    //data object for passing data from pregame scene
    public class ClientCharacterPlayerDataObject : NetworkBehaviour
    {
        public NetworkVariable<FixedString32Bytes> DisplayName;
        public NetworkVariable<int> AvatarIndex;
    }
}