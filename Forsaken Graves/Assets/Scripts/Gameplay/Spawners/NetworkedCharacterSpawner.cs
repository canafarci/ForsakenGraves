using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using ForsakenGraves.Gameplay.Character;
using ForsakenGraves.Gameplay.Messages;
using ForsakenGraves.Identifiers;
using ForsakenGraves.Infrastructure.Extensions;
using MessagePipe;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;
using VContainer;
using Random = UnityEngine.Random;

namespace ForsakenGraves.Gameplay.Spawners
{
    public class NetworkedCharacterSpawner : NetworkBehaviour
    {
        [SerializeField] private NetworkObject _characterToSpawn;
        [SerializeField] private Transform _spawnPoint;
        [SerializeField] private  float _spawnRate = 5f;

        [Inject] private IPublisher<CharacterSpawnedMessage> _characterSpawnedMessagePublisher;
        [Inject] private ServerCharacterSpawnState _serverCharacterSpawnState;
        
        private float _timeSinceSpawn;
        private bool _hasSpawned = false;

        public override void OnNetworkSpawn()
        {
            if (!IsServer)
            {
                enabled = false;
                return;
            }

            _timeSinceSpawn = _spawnRate;
            NetworkManager.SceneManager.OnLoadEventCompleted += OnLoadEventCompleteHandler;
        }

        private void OnLoadEventCompleteHandler(string scenename, LoadSceneMode loadscenemode, List<ulong> clientscompleted, List<ulong> clientstimedout)
        {
            _hasSpawned = true;
        }

        private void Update()
        {
            if (!_hasSpawned || _timeSinceSpawn < _spawnRate || !_serverCharacterSpawnState.CanSpawn)
            {
                _timeSinceSpawn += Time.deltaTime;
                return;
            }

            _timeSinceSpawn = 0f;
            
            SpawnNetworkCharacter();
        }

        private void SpawnNetworkCharacter()
        {
            Vector3 spawnPos = _spawnPoint.position;
            Vector3 randomizedSpawnPos = new Vector3(spawnPos.x + Random.value * 5f, 0f, spawnPos.z + Random.value * 5f);
            
            NetworkObject clone = Instantiate(_characterToSpawn, randomizedSpawnPos, Quaternion.identity);
            
            clone.Configure(); //initializes network variables before spawn
            clone.Spawn(true);

            CharacterTypes characterType = clone.GetComponent<ServerCharacter>().CharacterType;
            _characterSpawnedMessagePublisher.Publish(new CharacterSpawnedMessage(characterType, clone.gameObject));
        }

        public override void OnNetworkDespawn()
        {
            if (IsServer)
                NetworkManager.SceneManager.OnLoadEventCompleted -= OnLoadEventCompleteHandler;
        }
    }
}