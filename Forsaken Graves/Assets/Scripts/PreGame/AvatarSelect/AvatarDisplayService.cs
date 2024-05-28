using System;
using System.Collections.Generic;
using ForsakenGraves.Connection;
using ForsakenGraves.GameState;
using ForsakenGraves.Infrastructure.Netcode;
using ForsakenGraves.Infrastructure.Templates;
using ForsakenGraves.PreGame.Data;
using MessagePipe;
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

        public override void Initialize()
        {
            base.Initialize();
        }

        public override void ListenToMessages()
        {
            _networkSpawnMessageSubscriber.Subscribe(OnNetworkSpawnMessage).AddTo(_bag);
            _networkDespawnMessageSubscriber.Subscribe(OnNetworkDespawnMessage).AddTo(_bag);
        }

        private void OnNetworkSpawnMessage(OnNetworkSpawnMessage message)
        {
            Transform localAvatarTransform = _view.LocalClientAvatarDisplayView.AvatarHolderTransform;
            int avatarIndex = _model.AvatarIndex;
            GameObject avatarPrefab = _avatarsSO.PlayerAvatars[avatarIndex];

            _currentLocalAvatar = GameObject.Instantiate(avatarPrefab, localAvatarTransform);
            
            //avatar index is different from default LobbyPlayerData value, so update it on the server
            if (avatarIndex != 0)
            {
                _preGameNetwork.ChangeAvatarServerRpc(avatarIndex);
            }
        }

        public void SpawnPlayerAvatar(PlayerLobbyData playerLobbyData)
        {
            
        }
        
        private void OnNetworkDespawnMessage(OnNetworkDespawnMessage message)
        {
            
        }

        public override void Dispose()
        {
            base.Dispose();
        }
    }
}