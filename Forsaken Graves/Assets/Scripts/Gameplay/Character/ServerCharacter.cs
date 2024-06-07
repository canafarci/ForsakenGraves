using ForsakenGraves.Gameplay.Character.Stats;
using ForsakenGraves.Gameplay.Messages;
using ForsakenGraves.Identifiers;
using MessagePipe;
using Unity.Netcode;
using UnityEngine;
using VContainer;

namespace ForsakenGraves.Gameplay.Character
{
    //holds server side RPC methods for server logic
    public class ServerCharacter : NetworkBehaviour
    {
        [Inject] private NetworkCharacterHealth _networkCharacterHealth;
        [Inject] private IPublisher<CharacterDiedMessage> _characterDiedMessagePublisher;
        
        [SerializeField] private CharacterTypes _characterType;
        public CharacterTypes CharacterType => _characterType;

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
            Debug.Log($"Server received call for {damage} damage on GO {gameObject.name}");
            _networkCharacterHealth.CharacterHealth.Value -= damage;

            if (_networkCharacterHealth.CharacterHealth.Value <= 0f)
            {
                Debug.Log($"Server CALLED DESPAWN");
                KillCharacter();
            }
        }

        private void KillCharacter()
        {
            _characterDiedMessagePublisher.Publish(new CharacterDiedMessage(gameObject, CharacterType));
            DespawnObject();
        }

        private void DespawnObject()
        {
            NetworkObject networkObject = GetComponent<NetworkObject>();
            
            if (networkObject.IsSpawned)
                networkObject.Despawn();
        }

        public override void OnNetworkDespawn()
        {
            if (!IsServer) return;
            
            _networkCharacterHealth.OnCharacterDamaged -= NetworkCharacterDamagedHandler;
        }
        
        [Rpc(SendTo.Server)]
        public void DamageTargetServerRpc(NetworkObjectReference networkObjectReference, float damage)
        {
            NetworkObject networkObject = networkObjectReference;
            if (networkObject == null) return; //character reference is already despawned
            
            if (networkObject.TryGetComponent(out ITargetable targetable))
            {
                targetable.Damage(damage);
            }
            else
            {
                Debug.LogError("DAMAGE RPC HAS BEEN CALLED BUT NO ITargetable interface has been found!");
            }
        }
    }
}