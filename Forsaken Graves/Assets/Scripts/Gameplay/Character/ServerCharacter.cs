using ForsakenGraves.Gameplay.Character.Stats;
using Unity.Netcode;
using UnityEngine;

namespace ForsakenGraves.Gameplay.Character
{
    //holds server side RPC methods for server logic
    public class ServerCharacter : NetworkBehaviour
    {
        [SerializeField] private NetworkCharacterHealth _networkCharacterHealth;
        
        public override void OnNetworkSpawn()
        {
            if (!IsServer)
            {
                enabled = false;
                return;
            }

            _networkCharacterHealth.OnCharacterDamaged += NetworkCharacterDamagedHandler;
        }

        private void NetworkCharacterDamagedHandler(float damage)
        {
            _networkCharacterHealth.Health.Value -= damage;

            if (_networkCharacterHealth.Health.Value <= 0f)
            {
                DespawnObject();
            }
        }

        private void DespawnObject()
        {
            NetworkObject networkObject = GetComponent<NetworkObject>();
            networkObject.Despawn();
        }

        public override void OnNetworkDespawn()
        {
            if (!IsServer) return;
            
            _networkCharacterHealth.OnCharacterDamaged -= NetworkCharacterDamagedHandler;
        }
    }
}