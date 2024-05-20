using System;
using MessagePipe;
using VContainer;
using VContainer.Unity;

namespace ForsakenGraves.Test
{
    public class CrossScenePresenter : IInitializable, IDisposable
    {
        private readonly CrossSceneView _view;
        private readonly CrossSceneService _service;
        [Inject] private ISubscriber<int> _subscriberint;
        private IDisposable _disposableBag;


        public CrossScenePresenter(CrossSceneView view, CrossSceneService service)
        {
            _view = view;
            _service = service;
        }

        public void Initialize()
        {
            DisposableBagBuilder bag = DisposableBag.CreateBuilder(); // composite disposable for manage subscription
        
            _subscriberint.Subscribe(x => _service.Print(x.ToString() + "from infra")).AddTo(bag);

            _disposableBag = bag.Build();
        }

        public void Dispose()
        {
            _disposableBag?.Dispose();
        }
    }
}