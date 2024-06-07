using System.Collections.Generic;
using ForsakenGraves.Gameplay.Data;
using ForsakenGraves.Gameplay.Messages;
using ForsakenGraves.GameState;
using ForsakenGraves.Identifiers;
using ForsakenGraves.Infrastructure.Templates;
using MessagePipe;
using UnityEngine;
using VContainer;

namespace ForsakenGraves.Gameplay.Spawners
{
    public class ServerCharacterSpawnState : MessageSubscriberNetworkBehaviour
    {
        [Inject] private ISubscriber<CharacterDiedMessage> _characterDiedMessageSubscriber;
        [Inject] private ISubscriber<CharacterSpawnedMessage> _characterSpawnedMessageSubscriber;
        
        [Inject] private ServerGameplaySceneState _serverGameplaySceneState;
        [Inject] private GameplaySceneConfig _gameplaySceneConfig;

        private int _enemiesDied = 0;
        private int _enemiesToSpawn;
        private List<GameObject> _playerCharacters = new();

        public bool CanSpawn => _enemiesToSpawn > 0;
        public List<GameObject> PlayerCharacters => _playerCharacters;
        
        public override void OnNetworkSpawn()
        {
            base.OnNetworkSpawn();
            
            if (!IsServer)
            {
                enabled = false;
                return;
            }

            _enemiesToSpawn = _gameplaySceneConfig.EnemyCountToSpawn;
        }

        public override void ListenToMessages()
        { 
            if (!IsServer) return;
            
            _characterDiedMessageSubscriber.Subscribe(OnCharacterDiedMessage).AddTo(_bag);
            _characterSpawnedMessageSubscriber.Subscribe(OnCharacterSpawnedMessage).AddTo(_bag);
        }
        
        private void OnCharacterSpawnedMessage(CharacterSpawnedMessage message)
        {
            if (message.CharacterType is CharacterTypes.Player)
            {
                _playerCharacters.Add(message.CharacterObject);
            }
            else
            {
                _enemiesToSpawn--;
            }
        }
        
        private void OnCharacterDiedMessage(CharacterDiedMessage message)
        {
            if (message.CharacterType is CharacterTypes.Player)
            {
                _playerCharacters.RemoveAll((x) => x == null);
            
                if (message.CharacterObject != null)
                    _playerCharacters.Remove(message.CharacterObject);
            }
            else
            {
                _enemiesDied++;
                CheckGameOver();
            }
            
        }

        private void CheckGameOver()
        {
            if (_enemiesToSpawn <= 0 && _enemiesDied >= _gameplaySceneConfig.EnemyCountToSpawn)
            {
                _serverGameplaySceneState.OnGameOver(gameWon: true);
            }
        }
    }
}