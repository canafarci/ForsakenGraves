using System;
using MessagePipe;
using VContainer.Unity;

namespace ForsakenGraves.Infrastructure.Templates
{
    public abstract class MessageSubscriberTemplate : IInitializable, IDisposable
    {
        private IDisposable _disposableBag;
        protected DisposableBagBuilder _bag;

        public virtual void Initialize()
        {
            _bag = DisposableBag.CreateBuilder();
            ListenToMessages();
            _disposableBag = _bag.Build();
        }

        public abstract void ListenToMessages();
        
        public virtual void Dispose()
        {
            _disposableBag?.Dispose();
        }

    }
}