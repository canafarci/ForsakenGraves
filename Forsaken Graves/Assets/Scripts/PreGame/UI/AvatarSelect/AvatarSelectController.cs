using System;
using ForsakenGraves.Infrastructure.Netcode;
using ForsakenGraves.Infrastructure.Templates;
using ForsakenGraves.PreGame.AvatarSelect;
using ForsakenGraves.PreGame.Data;
using MessagePipe;
using Unity.Netcode;
using VContainer.Unity;

namespace ForsakenGraves.PreGame.UI.AvatarSelect
{
    public class AvatarSelectController : MessageSubscriberTemplate
    {
        private readonly AvatarSelectMediator _mediator;
        private readonly AvatarSelectModel _model;
        private readonly PlayerAvatarsSO _playerAvatarsSO;
        private readonly AvatarDisplayService _avatarDisplayService;
        private readonly PreGameNetwork _preGameNetwork;
        private readonly ISubscriber<OnNetworkSpawnMessage> _networkSpawnMessageSubscriber;
        private readonly ISubscriber<OnNetworkDespawnMessage> _networkDespawnMessageSubscriber;

        public AvatarSelectController(AvatarSelectMediator mediator,
                                      AvatarSelectModel model,
                                      PlayerAvatarsSO playerAvatarsSO,
                                      PreGameNetwork preGameNetwork,
                                      ISubscriber<OnNetworkSpawnMessage> networkSpawnMessageSubscriber,
                                      ISubscriber<OnNetworkDespawnMessage> networkDespawnMessageSubscriber)
        {
            _mediator = mediator;
            _model = model;
            _playerAvatarsSO = playerAvatarsSO;
            _preGameNetwork = preGameNetwork;
            _networkSpawnMessageSubscriber = networkSpawnMessageSubscriber;
            _networkDespawnMessageSubscriber = networkDespawnMessageSubscriber;
        }

        public override void Initialize()
        {
            base.Initialize();
            _mediator.OnAvatarChangeButtonClicked += AvatarChangeButtonClickedHandler;
        }

        public override void ListenToMessages()
        {
            _networkSpawnMessageSubscriber.Subscribe(OnNetworkSpawnMessage).AddTo(_bag);
            _networkDespawnMessageSubscriber.Subscribe(OnNetworkDespawnMessage).AddTo(_bag);
        }

        public void OnNetworkSpawnMessage(OnNetworkSpawnMessage message)
        {
            _preGameNetwork.PlayerLobbyDataNetworkList.OnListChanged += PlayerNetworkDataListChangedHandler;
        }
        public void OnNetworkDespawnMessage(OnNetworkDespawnMessage message)
        {
            _preGameNetwork.PlayerLobbyDataNetworkList.OnListChanged -= PlayerNetworkDataListChangedHandler;
        }
        
        private void PlayerNetworkDataListChangedHandler(NetworkListEvent<PlayerLobbyData> changeEvent)
        {
            if (changeEvent.Value.ClientID != NetworkManager.Singleton.LocalClient.ClientId) return;
            _mediator.EnableButton();
        }

        private void AvatarChangeButtonClickedHandler()
        {
            if (_preGameNetwork.IsLobbyLocked.Value) return;
            
            int currentIndex = _model.AvatarIndex;
            int avatarsLength = _playerAvatarsSO.PlayerAvatars.Count;

            int nextIndex = currentIndex + 1 == avatarsLength ? 0 : currentIndex + 1;
            
            _model.ChangeAvatarIndex(nextIndex);
            _preGameNetwork.ChangeAvatarServerRpc(nextIndex);
        }

        public override void Dispose()
        {
            base.Dispose();
            
            _mediator.OnAvatarChangeButtonClicked -= AvatarChangeButtonClickedHandler;
        }
    }
}