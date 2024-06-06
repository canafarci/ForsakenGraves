using ForsakenGraves.PreGame.Data;
using Unity.Netcode;
using UnityEngine;

namespace ForsakenGraves.Gameplay.Character.Player
{
    public class PlayerCharacterGraphicsSpawner : NetworkBehaviour
    {
        [SerializeField] private PlayerAvatarsSO _avatarsSO;
        [SerializeField] private ClientCharacterPlayerDataObject _clientCharacterPlayerDataObject;
        [SerializeField] private Transform _avatarParent;

        private bool _avatarSpawned = false;

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
        }
    }
}