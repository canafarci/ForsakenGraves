using ForsakenGraves.PreGame.Data;
using Unity.Netcode;
using UnityEngine;
using VContainer;

namespace ForsakenGraves.Gameplay.Character
{
    public class PlayerCharacterGraphicsSpawner : NetworkBehaviour
    {
        [SerializeField] private PlayerAvatarsSO _avatarsSO;
        [SerializeField] private ClientCharacterPlayerDataObject _clientCharacterPlayerDataObject;
        [SerializeField] private Transform _avatarParent;

        private bool _avatarSpawned = false;

        public override void OnNetworkSpawn()
        {
            if (!IsOwner || _avatarSpawned) return;

            SpawnAvatar();
        }

        private void SpawnAvatar()
        {
            int avatarIndex = _clientCharacterPlayerDataObject.AvatarIndex;
            Instantiate(_avatarsSO.PlayerAvatars[avatarIndex], _avatarParent);

            _avatarSpawned = true;
        }
    }
}