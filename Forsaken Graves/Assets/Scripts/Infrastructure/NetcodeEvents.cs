using ForsakenGraves.Infrastructure.Netcode;
using MessagePipe;
using Unity.Netcode;
using UnityEngine;
using VContainer;

namespace ForsakenGraves.Infrastructure
{
    public class NetcodeEvents : NetworkBehaviour
    {
        [Inject] private IPublisher<OnNetworkDespawnMessage> _despawnMessagePublisher;
        [Inject] private IPublisher<OnNetworkSpawnMessage> _spawnMessagePublisher;
        
        public override void OnNetworkSpawn()
        {
            _spawnMessagePublisher.Publish(new OnNetworkSpawnMessage());
        }

        public override void OnNetworkDespawn()
        {
            _despawnMessagePublisher.Publish(new OnNetworkDespawnMessage());
        }
    }
}