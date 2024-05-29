using ForsakenGraves.Gameplay.Data;
using Unity.Netcode;
using UnityEngine;

namespace ForsakenGraves.Gameplay.GameplayObjects
{
    public class PersistentPlayer : NetworkBehaviour
    {
        [SerializeField] private NetworkPlayerVisualData _networkPlayerVisualData;
        public NetworkPlayerVisualData PlayerVisualData => _networkPlayerVisualData;
    }
}