using ForsakenGraves.Identifiers;
using UnityEngine;

namespace ForsakenGraves.Gameplay.Messages
{
    public readonly struct CharacterDiedMessage
    {
        private readonly GameObject _characterObject;
        private readonly CharacterTypes _characterType;
        
        public CharacterTypes CharacterType => _characterType;
        public GameObject CharacterObject => _characterObject;

        public CharacterDiedMessage(GameObject characterObject, CharacterTypes characterType)
        {
            _characterObject = characterObject;
            _characterType = characterType;
        }
    }
}