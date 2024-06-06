using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;
using Random = UnityEngine.Random;

namespace ForsakenGraves.Gameplay.Spawners
{
    public class NetworkedCharacterSpawner : NetworkBehaviour
    {
        [SerializeField] private NetworkObject _characterToSpawn;
        [SerializeField] private Transform _spawnPoint;
        
        private const float SPAWN_RATE = 5f;
        private float _timeSinceSpawn = SPAWN_RATE;

        private bool _hasSpawned = false;

        public override void OnNetworkSpawn()
        {
            if (!IsServer)
            {
                enabled = false;
                return;
            }
            
            NetworkManager.SceneManager.OnLoadEventCompleted += OnLoadEventCompleteHandler;
        }

        private void OnLoadEventCompleteHandler(string scenename, LoadSceneMode loadscenemode, List<ulong> clientscompleted, List<ulong> clientstimedout)
        {
            _hasSpawned = true;
        }

        private void Update()
        {
            if (!_hasSpawned || _timeSinceSpawn < SPAWN_RATE)
            {
                _timeSinceSpawn += Time.deltaTime;
                return;
            }

            _timeSinceSpawn = 0f;
            
            Vector3 spawnPos = _spawnPoint.position;
            Vector3 randomizedSpawnPos = new Vector3(spawnPos.x + Random.value * 5f, 0f, spawnPos.z + Random.value * 5f);
            
            NetworkObject clone = Instantiate(_characterToSpawn, randomizedSpawnPos, Quaternion.identity);

            clone.Spawn(true);
        }

        public override void OnNetworkDespawn()
        {
            if (IsServer)
                NetworkManager.SceneManager.OnLoadEventCompleted -= OnLoadEventCompleteHandler;
        }
    }
}