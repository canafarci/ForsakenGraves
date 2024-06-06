using Unity.Collections;
using Unity.Netcode;

namespace ForsakenGraves.Gameplay.Character.Player
{
    //data object for passing data from pregame scene
    public class ClientCharacterPlayerDataObject : NetworkBehaviour
    {
        public NetworkVariable<FixedString32Bytes> DisplayName;
        public NetworkVariable<int> AvatarIndex;
    }
}