using Unity.Collections;
using Unity.Netcode;

namespace ForsakenGraves.Gameplay.Data
{
    public class NetworkPlayerVisualData : NetworkBehaviour
    {
        public NetworkVariable<FixedString32Bytes> DisplayName = new NetworkVariable<FixedString32Bytes>();
        public NetworkVariable<int> AvatarIndex = new NetworkVariable<int>();
    }
}