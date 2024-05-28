using System;
using ForsakenGraves.Connection;
using ForsakenGraves.PreGame.Signals;
using MessagePipe;
using VContainer;
using VContainer.Unity;

namespace ForsakenGraves.PreGame.UI.ReadyPanel
{
    public class ReadyPanelController : IInitializable, IDisposable
    {
        [Inject] private ISubscriber<PlayerReadyChangedMessage> _readyChangedSubscriber;
        [Inject] private ConnectionStateManager _connectionStateManager;
        
        private readonly ReadyPanelMediator _mediator;
        private IDisposable _disposableBag;

        public ReadyPanelController(ReadyPanelMediator mediator)
        {
            _mediator = mediator;
        }

        public void Initialize()
        {
            DisposableBagBuilder bag = DisposableBag.CreateBuilder();;
            _readyChangedSubscriber.Subscribe(OnPlayerReadyChanged).AddTo(bag);
            _disposableBag = bag.Build();
        }

        private void OnPlayerReadyChanged(PlayerReadyChangedMessage message)
        {
            ulong clientId = _connectionStateManager.NetworkManager.LocalClient.ClientId;
            if (clientId != message.ClientID) return;

            _mediator.UpdateReadyButton(message.IsReady);
        }

        public void Dispose()
        {
            _disposableBag?.Dispose();
        }
    }
}