using UnityEngine;

namespace ForsakenGraves.Gameplay.Spawners
{
    public readonly struct  PlayerCharacterDespawnedMessage
    {
        private readonly GameObject _playerCharacter;
        public GameObject PlayerCharacter => _playerCharacter;

        public PlayerCharacterDespawnedMessage(GameObject playerCharacter)
        {
            _playerCharacter = playerCharacter;
        }
    }
}