using System;
using ForsakenGraves.Connection;
using ForsakenGraves.Infrastructure.Templates;
using ForsakenGraves.PreGame.Signals;
using MessagePipe;
using VContainer;

namespace ForsakenGraves.PreGame.UI.ReadyPanel
{
    public class ReadyPanelController : MessageSubscriberTemplate
    {
        [Inject] private ISubscriber<PlayerReadyChangedMessage> _readyChangedSubscriber;
        [Inject] private ConnectionStateManager _connectionStateManager;
        
        private readonly ReadyPanelMediator _mediator;
        private IDisposable _disposableBag;

        public ReadyPanelController(ReadyPanelMediator mediator)
        {
            _mediator = mediator;
        }

        public override void ListenToMessages()
        {
            _readyChangedSubscriber.Subscribe(OnPlayerReadyChanged).AddTo(_bag);
        }

        private void OnPlayerReadyChanged(PlayerReadyChangedMessage message)
        {
            ulong clientId = _connectionStateManager.NetworkManager.LocalClient.ClientId;
            if (clientId != message.ClientID) return;

            _mediator.UpdateReadyButton(message.IsReady);
        }
    }
}