using System;
using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using ForsakenGraves.Connection;
using ForsakenGraves.GameState;
using ForsakenGraves.Infrastructure.Netcode;
using ForsakenGraves.Infrastructure.Templates;
using ForsakenGraves.PreGame.Data;
using MessagePipe;
using Unity.Netcode;
using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace ForsakenGraves.PreGame.AvatarSelect
{
    public class AvatarDisplayService : MessageSubscriberTemplate
    {
        [Inject] private ISubscriber<OnNetworkSpawnMessage> _networkSpawnMessageSubscriber;
        [Inject] private ISubscriber<OnNetworkDespawnMessage> _networkDespawnMessageSubscriber;
        
        private readonly ServerPreGameState _serverPreGameState;
        private readonly PreGameNetwork _preGameNetwork;
        private readonly PlayerAvatarsSO _avatarsSO;
        private readonly AvatarSelectModel _model;
        private readonly AvatarDisplayCompositeView _view;

        private GameObject _currentLocalAvatar;
        private List<GameObject> _otherPlayerAvatars = new();

        public AvatarDisplayService(ServerPreGameState serverPreGameState,
                                    PreGameNetwork preGameNetwork,
                                    PlayerAvatarsSO avatarsSO,
                                    AvatarSelectModel model,
                                    AvatarDisplayCompositeView view)
        {
            _serverPreGameState = serverPreGameState;
            _preGameNetwork = preGameNetwork;
            _avatarsSO = avatarsSO;
            _model = model;
            _view = view;
        }

        public override void ListenToMessages()
        {
            _networkSpawnMessageSubscriber.Subscribe(OnNetworkSpawnMessage).AddTo(_bag);
            _networkDespawnMessageSubscriber.Subscribe(OnNetworkDespawnMessage).AddTo(_bag);
        }

        private void OnNetworkSpawnMessage(OnNetworkSpawnMessage message)
        {
            _preGameNetwork.PlayerLobbyDataNetworkList.OnListChanged += NetworkListChangedHandler;
            
            int avatarIndex = _model.AvatarIndex;
            _preGameNetwork.ChangeAvatarServerRpc(avatarIndex);
        }

        private async void NetworkListChangedHandler(NetworkListEvent<PlayerLobbyData> changeEvent)
        {
            CleanOldAvatars();
            
            //wait until all avatars are cleaned
            await UniTask.WaitUntil(() => _view.AvatarDisplayViews[0].AvatarHolderTransform.childCount == 0);

            NetworkList<PlayerLobbyData> playerLobbyDataNetworkList = _preGameNetwork.PlayerLobbyDataNetworkList;
            ulong clientID = NetworkManager.Singleton.LocalClient.ClientId;
            
            for (int i = 0; i < playerLobbyDataNetworkList.Count; i++)
            {
                PlayerLobbyData lobbyData = playerLobbyDataNetworkList[i];
                if (lobbyData.ClientID == clientID)
                    SpawnLocalPlayerAvatar(lobbyData);
                else
                    SpawnOtherPlayerAvatar(lobbyData);
            }
        }

        private void CleanOldAvatars()
        {
            //Clear present avatar GOs
            //NOTE: this can be optimized by checking if avatars have changed
            //or only updating the changes
            //or pre-spawning avatars and changing activation
            GameObject.Destroy(_currentLocalAvatar);
            _otherPlayerAvatars.ForEach(GameObject.Destroy);
            _otherPlayerAvatars.Clear();
        }

        private void SpawnOtherPlayerAvatar(PlayerLobbyData playerLobbyData)
        {
            Transform otherPlayerTransform = _view.AvatarDisplayViews
                                                  .FirstOrDefault(x => x.AvatarHolderTransform.childCount == 0)
                                                  ?.AvatarHolderTransform;
            
            GameObject otherPlayerAvatar = SpawnAvatar(playerLobbyData, otherPlayerTransform);
            _otherPlayerAvatars.Add(otherPlayerAvatar);
        }

        private void SpawnLocalPlayerAvatar(PlayerLobbyData playerLobbyData)
        {
            Transform localAvatarTransform = _view.LocalClientAvatarDisplayView.AvatarHolderTransform;
            _currentLocalAvatar = SpawnAvatar(playerLobbyData, localAvatarTransform);
        }
        
        private GameObject SpawnAvatar(PlayerLobbyData playerLobbyData, Transform spawnTransform)
        {
            int avatarIndex = playerLobbyData.AvatarIndex;
            GameObject avatarPrefab = _avatarsSO.PlayerAvatars[avatarIndex];
            
            GameObject otherPlayerAvatar =  GameObject.Instantiate(avatarPrefab, spawnTransform);
            return otherPlayerAvatar;
        }
        
        private void OnNetworkDespawnMessage(OnNetworkDespawnMessage message)
        {
            _preGameNetwork.PlayerLobbyDataNetworkList.OnListChanged -= NetworkListChangedHandler;
        }
    }
}