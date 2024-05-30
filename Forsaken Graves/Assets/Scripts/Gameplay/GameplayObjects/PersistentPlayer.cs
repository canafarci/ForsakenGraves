using ForsakenGraves.Gameplay.Data;
using Unity.Netcode;
using UnityEngine;

namespace ForsakenGraves.Gameplay.GameplayObjects
{
    public class PersistentPlayer : NetworkBehaviour
    {
        [SerializeField] private NetworkPlayerVisualData _networkPlayerVisualData;
        public NetworkPlayerVisualData PlayerVisualData => _networkPlayerVisualData;

        public override void OnNetworkSpawn()
        {
            ulong clientID = NetworkManager.Singleton.LocalClient.ClientId;
            name = $"Client {clientID}'s Persistent Player";
        }
    }
}