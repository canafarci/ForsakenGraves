using System;
using ForsakenGraves.Connection;
using ForsakenGraves.Gameplay.State;
using ForsakenGraves.GameState;
using ForsakenGraves.Identifiers;
using ForsakenGraves.Infrastructure.Netcode;
using ForsakenGraves.Infrastructure.SceneManagement.Messages;
using ForsakenGraves.Infrastructure.Templates;
using MessagePipe;
using TMPro;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;
using VContainer;
using VContainer.Unity;

namespace ForsakenGraves.PostGame
{
    public class PostGamePanelController : MessageSubscriberTemplate
    {
        [Inject] private  IPublisher<LoadSceneMessage> _loadSceneMessagePublisher;
        [Inject] private ISubscriber<OnNetworkSpawnMessage> _onNetworkSpawnMessageSubscriber;
        [Inject] private ISubscriber<OnNetworkDespawnMessage> _OnNetworkDespawnMessageSubscriber;
        
        private readonly PostGamePanelMediator _mediator;
        private readonly PersistentGameplayState _persistentGameplayState;
        private readonly ServerPostGameState _serverPostGameState;
        private readonly ConnectionStateManager _connectionStateManager;

        public PostGamePanelController(PostGamePanelMediator mediator,
                                       PersistentGameplayState persistentGameplayState,
                                       ServerPostGameState serverPostGameState,
                                       ConnectionStateManager connectionStateManager)
        {
            _mediator = mediator;
            _persistentGameplayState = persistentGameplayState;
            _serverPostGameState = serverPostGameState;
            _connectionStateManager = connectionStateManager;
        }

        public override void Initialize()
        {
            base.Initialize();
            _mediator.OnMainMenuButtonClicked += MainMenuButtonClickedHandler;
            _mediator.OnReplayButtonClicked += OnReplayButtonClickedHandler;
        }

        public override void ListenToMessages()
        {
            _onNetworkSpawnMessageSubscriber.Subscribe(OnNetworkSpawnMessage).AddTo(_bag);
            _OnNetworkDespawnMessageSubscriber.Subscribe(OnNetworkDespawnMessage).AddTo(_bag);
        }
        
        private void OnNetworkDespawnMessage(OnNetworkDespawnMessage message)
        {
            
        }
        
        private void OnNetworkSpawnMessage(OnNetworkSpawnMessage message)
        {
            if (!NetworkManager.Singleton.IsServer)
                _mediator.DisableReplayButton();
        }

        private void OnReplayButtonClickedHandler()
        {
            _loadSceneMessagePublisher.Publish(new LoadSceneMessage(SceneIdentifier.PreGameScene, true));
        }

        private void MainMenuButtonClickedHandler()
        {
            _connectionStateManager.RequestShutdown();
        }

        public override void Dispose()
        {
            _mediator.OnMainMenuButtonClicked -= MainMenuButtonClickedHandler;
            _mediator.OnReplayButtonClicked -= OnReplayButtonClickedHandler;
            base.Dispose();
        }
    }
}