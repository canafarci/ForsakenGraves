using System.Collections.Generic;
using System.Linq;
using ForsakenGraves.GameState;
using ForsakenGraves.Infrastructure.Netcode;
using ForsakenGraves.Infrastructure.Templates;
using ForsakenGraves.PreGame.Data;
using MessagePipe;
using Unity.Netcode;
using UnityEngine;
using VContainer;

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

        private void NetworkListChangedHandler(NetworkListEvent<PlayerLobbyData> changeEvent)
        {
            if (_preGameNetwork.IsLobbyLocked.Value) return;
            
            NetworkList<PlayerLobbyData> playerLobbyDataNetworkList = _preGameNetwork.PlayerLobbyDataNetworkList;
            ulong clientID = NetworkManager.Singleton.LocalClient.ClientId;

            int otherPlayerIndex = 0;
            for (int i = 0; i < playerLobbyDataNetworkList.Count; i++)
            {
                PlayerLobbyData lobbyData = playerLobbyDataNetworkList[i];
                if (lobbyData.ClientID == clientID)
                    SpawnLocalPlayerAvatar(lobbyData);
                else
                    SpawnOtherPlayerAvatar(lobbyData, otherPlayerIndex++);
            }
        }
        
        private void SpawnOtherPlayerAvatar(PlayerLobbyData playerLobbyData, int index)
        {
            Transform otherPlayerTransform = _view.AvatarDisplayViews[index].AvatarHolderTransform;
            
            SpawnAvatar(playerLobbyData, otherPlayerTransform);
        }

        private void SpawnLocalPlayerAvatar(PlayerLobbyData playerLobbyData)
        {
            Transform localAvatarTransform = _view.LocalClientAvatarDisplayView.AvatarHolderTransform;
            SpawnAvatar(playerLobbyData, localAvatarTransform);
        }
        
        private void SpawnAvatar(PlayerLobbyData playerLobbyData, Transform spawnTransform)
        {
            if (spawnTransform.childCount > 0)
                for (int i = 0; i < spawnTransform.childCount; i++)
                    GameObject.Destroy(spawnTransform.GetChild(i).gameObject);
            
            int avatarIndex = playerLobbyData.AvatarIndex;
            GameObject avatarPrefab = _avatarsSO.OtherPlayerAvatars[avatarIndex];
            
            GameObject.Instantiate(avatarPrefab, spawnTransform);
        }
        
        private void OnNetworkDespawnMessage(OnNetworkDespawnMessage message)
        {
            _preGameNetwork.PlayerLobbyDataNetworkList.OnListChanged -= NetworkListChangedHandler;
        }
    }
}