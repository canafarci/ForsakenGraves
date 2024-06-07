using System;
using MessagePipe;
using Unity.Netcode;

namespace ForsakenGraves.Infrastructure.Templates
{
    public abstract class MessageSubscriberNetworkBehaviour : NetworkBehaviour
    {
        private IDisposable _disposableBag;
        protected DisposableBagBuilder _bag;

        public override void OnNetworkSpawn()
        {
            _bag = DisposableBag.CreateBuilder();
            ListenToMessages();
            _disposableBag = _bag.Build();
        }

        public abstract void ListenToMessages();

        public override void OnDestroy()
        {
            _disposableBag?.Dispose();
            base.OnDestroy();
        }
    }
}