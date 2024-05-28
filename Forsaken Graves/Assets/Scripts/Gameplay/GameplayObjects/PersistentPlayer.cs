using ForsakenGraves.UnityService.Data;
using Unity.Netcode;
using UnityEngine;

namespace ForsakenGraves.Gameplay.GameplayObjects
{
    public class PersistentPlayer : NetworkBehaviour
    {
        private PlayerData _playerData;
    }
}