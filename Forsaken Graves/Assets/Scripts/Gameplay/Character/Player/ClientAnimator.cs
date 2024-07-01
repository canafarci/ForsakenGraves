using System;
using Cysharp.Threading.Tasks;
using Cysharp.Threading.Tasks.Triggers;
using ForsakenGraves.Infrastructure.Netcode;
using Unity.Netcode;
using UnityEngine;
using VContainer;

namespace ForsakenGraves.Gameplay.Character.Player
{
    public class ClientAnimator : NetworkBehaviour
    {
        [Inject] private OwnerNetworkAnimator _ownerNetworkAnimator;
        [Inject] private ClientInventory _clientInventory;
        [Inject] private PlayerCharacterGraphicsSpawner _graphicsSpawner;

        private void Awake()
        {
            _graphicsSpawner.OnAvatarSpawned += AvatarSpawnedHandler;
        }
        
        public override void OnNetworkSpawn()
        {
            if (!IsOwner)
            {
                enabled = false;
                return;
            }
        }
        
        private  void AvatarSpawnedHandler() => _ownerNetworkAnimator.Animator.Rebind();

        public override void OnNetworkDespawn()
        {
            _graphicsSpawner.OnAvatarSpawned -= AvatarSpawnedHandler;
        }
    }
}