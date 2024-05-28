using System;
using Cysharp.Threading.Tasks;
using ForsakenGraves.Connection;
using ForsakenGraves.Infrastructure.Netcode;
using ForsakenGraves.Infrastructure.Templates;
using ForsakenGraves.PreGame.Data;
using MessagePipe;
using Unity.Netcode;
using VContainer;
using VContainer.Unity;

namespace ForsakenGraves.PreGame.UI.ReadyPanel
{
    public class ReadyPanelController : MessageSubscriberTemplate
    {
        [Inject] private ConnectionStateManager _connectionStateManager;
        [Inject] private PreGameNetwork _preGameNetwork;

        [Inject] private ISubscriber<OnNetworkSpawnMessage> _networkSpawnSubscriber;
        [Inject] private ISubscriber<OnNetworkDespawnMessage> _networkDespawnSubscriber;
        
        private readonly ReadyPanelMediator _mediator;

        public ReadyPanelController(ReadyPanelMediator mediator)
        {
            _mediator = mediator;
        }
        
        public override void ListenToMessages()
        {
            _networkSpawnSubscriber.Subscribe(OnNetworkSpawn).AddTo(_bag);
            _networkDespawnSubscriber.Subscribe(OnNetworkDespawn).AddTo(_bag);
        }

        private void OnNetworkSpawn(OnNetworkSpawnMessage message)
        {
            _preGameNetwork.PlayerLobbyDataNetworkList.OnListChanged += OnNetworkListChanged;
        }
        
        private void OnNetworkListChanged(NetworkListEvent<PlayerLobbyData> changedList)
        {
            ulong clientID = _connectionStateManager.NetworkManager.LocalClient.ClientId;
            //(int playerIndex, PlayerLobbyData lobbyData) clientData = _preGameNetwork.GetPlayerLobbyData(clientID);
            
            if (changedList.Value.ClientID != clientID) return;
            
            _mediator.UpdateReadyButton(changedList.Value.IsReady);
        }
        
        private void OnNetworkDespawn(OnNetworkDespawnMessage message)
        {
            _preGameNetwork.PlayerLobbyDataNetworkList.OnListChanged -= OnNetworkListChanged;
        }
    }
}