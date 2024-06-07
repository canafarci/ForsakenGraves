using System;
using System.Collections.Generic;
using ForsakenGraves.Gameplay.Character;
using ForsakenGraves.Gameplay.Character.Player;
using ForsakenGraves.Gameplay.Data;
using ForsakenGraves.Gameplay.GameplayObjects;
using ForsakenGraves.Gameplay.Spawners;
using ForsakenGraves.Infrastructure.Extensions;
using MessagePipe;
using NUnit.Framework;
using Unity.Collections;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;
using VContainer;

namespace ForsakenGraves.GameState
{
    public class ServerGameplaySceneState: NetworkBehaviour
    {
        [SerializeField] private NetworkObject _playerPrefab;
        [Inject] private ISubscriber<PlayerCharacterDespawnedMessage> _playerDespawnedSubscriber;
        
        private bool _isInitialSpawnDone = false;
        private List<GameObject> _playerCharacters = new();
        public List<GameObject> PlayerCharacters => _playerCharacters;

        private IDisposable _disposables;

        public override void OnNetworkSpawn()
        {
            if (!IsServer)
            {
                enabled = false;
                return;
            }

            SubscribeToMessages();

            NetworkManager.Singleton.SceneManager.OnLoadEventCompleted += OnLoadEventCompleted;

        }

        private void SubscribeToMessages()
        {
            DisposableBagBuilder bag = DisposableBag.CreateBuilder();
            
            _playerDespawnedSubscriber.Subscribe(OnPlayerDespawned).AddTo(bag);

            _disposables = bag.Build();
        }

        private void OnPlayerDespawned(PlayerCharacterDespawnedMessage message) =>
            _playerCharacters.Remove(message.PlayerCharacter);

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
                InitialSpawnPlayer(clients.Key);
            }
        }

        private void InitialSpawnPlayer(ulong clientID)
        {
            NetworkObject playerNetworkObject = NetworkManager.Singleton.SpawnManager.GetPlayerNetworkObject(clientID);
            NetworkObject newPlayer = Instantiate(_playerPrefab, Vector3.zero, Quaternion.identity);
            ServerPlayerCharacter newPlayerPlayerCharacter = newPlayer.GetComponent<ServerPlayerCharacter>();
            
            bool persistentPlayerExists = playerNetworkObject.TryGetComponent(out PersistentPlayer persistentPlayer);
            Assert.IsTrue(persistentPlayerExists,  $"Persistent player for {clientID} is not present!");
            
            bool playerDataObjectExists = newPlayer.TryGetComponent(out ClientCharacterPlayerDataObject playerDataObject);
            Assert.IsTrue(playerDataObjectExists,  $"ClientCharacterPlayerDataObject for {clientID} is not present!");

            playerDataObject.DisplayName = new NetworkVariable<FixedString32Bytes>(persistentPlayer.PlayerVisualData.DisplayName.Value);
            playerDataObject.AvatarIndex = new NetworkVariable<int>( persistentPlayer.PlayerVisualData.AvatarIndex.Value);
            
            newPlayer.Configure();
            newPlayer.SpawnWithOwnership(clientID, true);
            
            _playerCharacters.Add(newPlayer.gameObject);
        }
#endregion
        

        public override void OnNetworkDespawn()
        {
            if (!IsServer) return;
            
            NetworkManager.Singleton.SceneManager.OnLoadEventCompleted -= OnLoadEventCompleted;
        }

        public override void OnDestroy()
        {
            if (IsServer)
            {
                _disposables.Dispose();
            }
            
            base.OnDestroy();
        }
    }
}