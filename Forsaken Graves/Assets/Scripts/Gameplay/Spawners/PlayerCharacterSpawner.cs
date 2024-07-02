using System.Collections.Generic;
using ForsakenGraves.Gameplay.Character.Player;
using ForsakenGraves.Gameplay.GameplayObjects;
using ForsakenGraves.Gameplay.Messages;
using ForsakenGraves.Identifiers;
using ForsakenGraves.Infrastructure.Extensions;
using MessagePipe;
using NUnit.Framework;
using Unity.Collections;
using Unity.Netcode;
using UnityEngine;
using VContainer;

namespace ForsakenGraves.Gameplay.Spawners
{
    public class PlayerCharacterSpawner : NetworkBehaviour
    {
        [Inject] private IPublisher<CharacterSpawnedMessage> _characterSpawnedMessagePublisher;
        [SerializeField] private NetworkObject _playerPrefab;
        [SerializeField] private List<Transform> _spawnTransforms;

        private int _spawnIndex = 0;
        
        public void SpawnPlayerCharacter(ulong clientID)
        {
            NetworkObject playerNetworkObject = NetworkManager.Singleton.SpawnManager.GetPlayerNetworkObject(clientID);
            NetworkObject newPlayer = Instantiate(_playerPrefab, Vector3.zero, Quaternion.identity);

            Vector3 spawnPos = _spawnTransforms[_spawnIndex++].position;
            newPlayer.transform.position = spawnPos;
            
            ServerPlayerCharacter newPlayerPlayerCharacter = newPlayer.GetComponent<ServerPlayerCharacter>();
            
            bool persistentPlayerExists = playerNetworkObject.TryGetComponent(out PersistentPlayer persistentPlayer);
            Assert.IsTrue(persistentPlayerExists,  $"Persistent player for {clientID} is not present!");
            
            bool playerDataObjectExists = newPlayer.TryGetComponent(out ClientCharacterPlayerDataObject playerDataObject);
            Assert.IsTrue(playerDataObjectExists,  $"ClientCharacterPlayerDataObject for {clientID} is not present!");

            playerDataObject.DisplayName = new NetworkVariable<FixedString32Bytes>(persistentPlayer.PlayerVisualData.DisplayName.Value);
            playerDataObject.AvatarIndex = new NetworkVariable<int>( persistentPlayer.PlayerVisualData.AvatarIndex.Value);
            

            newPlayer.Configure();
            newPlayer.SpawnWithOwnership(clientID, true);
            
            _characterSpawnedMessagePublisher.Publish(new CharacterSpawnedMessage(CharacterTypes.Player, newPlayer.gameObject));
        }
    }
}