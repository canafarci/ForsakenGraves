using System;
using ForsakenGraves.PreGame.Data;
using Unity.Netcode;
using UnityEngine;
using VContainer;

namespace ForsakenGraves.Gameplay.Character.Player
{
    public class PlayerCharacterGraphicsSpawner : NetworkBehaviour
    {
        [Inject] private PlayerAvatarsSO _avatarsSO;
        [Inject] private ClientCharacterPlayerDataObject _clientCharacterPlayerDataObject;
        
        [SerializeField] private Transform _avatarParent;

        private bool _avatarSpawned = false;

        public event Action OnAvatarSpawned;

        public override void OnNetworkSpawn()
        {
            if (!IsClient || _avatarSpawned) return;

            SpawnAvatar();
        }

        private void SpawnAvatar()
        {
            int avatarIndex = _clientCharacterPlayerDataObject.AvatarIndex.Value;
            
            GameObject avatarPrefab = IsOwner ? _avatarsSO.PlayableAvatars[avatarIndex] : _avatarsSO.OtherPlayerAvatars[avatarIndex]; 
            Instantiate(avatarPrefab, _avatarParent);

            _avatarSpawned = true;
            OnAvatarSpawned?.Invoke();
        }
    }
}