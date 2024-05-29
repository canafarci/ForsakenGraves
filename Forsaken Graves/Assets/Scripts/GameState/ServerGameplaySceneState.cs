using System.Collections.Generic;
using ForsakenGraves.Gameplay.Character;
using ForsakenGraves.Gameplay.Data;
using ForsakenGraves.Gameplay.GameplayObjects;
using NUnit.Framework;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace ForsakenGraves.GameState
{
    public class ServerGameplaySceneState: NetworkBehaviour
    {
        [SerializeField] private NetworkObject _playerPrefab;
        
        private bool _isInitialSpawnDone = false;
        
        public override void OnNetworkSpawn()
        {
            if (!IsServer)
            {
                enabled = false;
                return;
            }
            
            NetworkManager.Singleton.SceneManager.OnLoadEventCompleted += OnLoadEventCompleted;

        }

#region Load Event & Spawn Players
        private void OnLoadEventCompleted(string sceneName, LoadSceneMode loadSceneMode, List<ulong> clientsCompleted, List<ulong> clientsTimedOut)
        {
            InitialSpawn(loadSceneMode);
        }

        private void InitialSpawn(LoadSceneMode loadSceneMode)
        {
            if (_isInitialSpawnDone || loadSceneMode != LoadSceneMode.Single ) return;

            foreach (KeyValuePair<ulong, NetworkClient> clients in NetworkManager.Singleton.ConnectedClients)
            {
                InitialSpawnPlayer(clients.Key);
            }
        }

        private void InitialSpawnPlayer(ulong clientID)
        {
            NetworkObject playerNetworkObject = NetworkManager.Singleton.SpawnManager.GetPlayerNetworkObject(clientID);
            NetworkObject newPlayer = Instantiate(_playerPrefab, Vector3.zero, Quaternion.identity);
            ServerCharacter newPlayerCharacter = newPlayer.GetComponent<ServerCharacter>();
            
            bool persistentPlayerExists = playerNetworkObject.TryGetComponent(out PersistentPlayer persistentPlayer);
            Assert.IsTrue(persistentPlayerExists,  $"Persistent player for {clientID} is not present!");

                //NetworkPlayerVisualData playerVisualData = persistentPlayer.PlayerVisualData;
            
            newPlayer.SpawnWithOwnership(clientID, true);
        }
#endregion
        

        public override void OnNetworkDespawn()
        {
            if (!IsServer) return;
            
            NetworkManager.Singleton.SceneManager.OnLoadEventCompleted -= OnLoadEventCompleted;
        }
    }
}