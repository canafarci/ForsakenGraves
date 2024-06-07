using System;
using System.Collections.Generic;
using ForsakenGraves.Gameplay.Messages;
using ForsakenGraves.Gameplay.Spawners;
using ForsakenGraves.Gameplay.State;
using ForsakenGraves.Identifiers;
using ForsakenGraves.Infrastructure.SceneManagement.Messages;
using ForsakenGraves.Infrastructure.Templates;
using MessagePipe;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;
using VContainer;

namespace ForsakenGraves.GameState
{
    public class ServerGameplaySceneState: MessageSubscriberNetworkBehaviour
    {
        [Inject] private IPublisher<LoadSceneMessage> _loadSceneMessagePublisher;
        
        [Inject] private ISubscriber<CharacterSpawnedMessage> _characterSpawnedMessageSubscriber;
        [Inject] private ISubscriber<CharacterDiedMessage> _characterDiedMessageSubscriber;

        [Inject] private PersistentGameplayState _persistentGameplayState;
        [Inject] private PlayerCharacterSpawner _playerCharacterSpawner;        
        
        private bool _isInitialSpawnDone = false;
        
        public override void OnNetworkSpawn()
        {
            base.OnNetworkSpawn();
            
            if (!IsServer)
            {
                enabled = false;
                return;
            }
            
            NetworkManager.Singleton.SceneManager.OnLoadEventCompleted += OnLoadEventCompleted;
        }
        
        public override void ListenToMessages()
        {
            if (!IsServer) return;
            
        }
        
        #region Load Event & Spawn Players
        private void OnLoadEventCompleted(string sceneName, LoadSceneMode loadSceneMode, List<ulong> clientsCompleted, List<ulong> clientsTimedOut)
        {
            InitialSpawn(loadSceneMode);
        }

        private void InitialSpawn(LoadSceneMode loadSceneMode)
        {
            if (_isInitialSpawnDone || loadSceneMode != LoadSceneMode.Single ) return;

            _isInitialSpawnDone = true;
            foreach (KeyValuePair<ulong, NetworkClient> clients in NetworkManager.Singleton.ConnectedClients)
            {
                _playerCharacterSpawner.SpawnPlayerCharacter(clients.Key);
            }
        }
#endregion

#region Gameplay Cycle
        public void OnGameOver(bool gameWon)
        {
            _persistentGameplayState.IsGameWon = gameWon;
            _loadSceneMessagePublisher.Publish(new LoadSceneMessage(sceneID: SceneIdentifier.PostGameScene, true));
        }
#endregion

        public override void OnNetworkDespawn()
        {
            if (!IsServer) return;
            
            NetworkManager.Singleton.SceneManager.OnLoadEventCompleted -= OnLoadEventCompleted;
        }
    }
}