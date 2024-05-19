using System;
using MessagePipe;
using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace ForsakenGraves.Test
{
    public class TestSubscriber : IDisposable, IInitializable
    {
        private readonly TestService _service;
        
        [Inject] private ISubscriber<string> _subscriber;
        private IDisposable _disposableBag;

        public TestSubscriber(TestService service)
        {
            _service = service;
        }
        
        public void Initialize()
        {
            DisposableBagBuilder bag = DisposableBag.CreateBuilder(); // composite disposable for manage subscription
        
            _subscriber.Subscribe(x => _service.Print(x)).AddTo(bag);

            _disposableBag = bag.Build();
        }
        
        public void Dispose()
        {
            _disposableBag.Dispose();
            Debug.Log("----------");
        }
    }
}