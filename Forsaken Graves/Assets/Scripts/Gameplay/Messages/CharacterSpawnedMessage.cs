using ForsakenGraves.Identifiers;
using UnityEngine;

namespace ForsakenGraves.Gameplay.Messages
{
    public readonly struct CharacterSpawnedMessage
    {
        private readonly CharacterTypes _characterType;
        private readonly GameObject _characterObject;

        public CharacterTypes CharacterType => _characterType;
        public GameObject CharacterObject => _characterObject;
        
        public CharacterSpawnedMessage(CharacterTypes characterType, GameObject characterObject)
        {
            _characterType = characterType;
            _characterObject = characterObject;
        }

    }
}