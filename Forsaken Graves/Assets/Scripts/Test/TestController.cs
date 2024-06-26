using MessagePipe;
using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace ForsakenGraves.Test
{
    public class TestController : IStartable
    {
        private readonly TestView _view;
        private readonly TestService _service;
        private readonly CrossSceneService _crossSceneService;
        private readonly CrossSceneView _crossSceneView;

        [Inject] private IPublisher<string> _publisher;
        [Inject] private IPublisher<int> _publisherint;

        public TestController(TestView view,
                              TestService service,
                              CrossSceneService crossSceneService,
                              CrossSceneView crossSceneView)
        {
            _view = view;
            _service = service;
            _crossSceneService = crossSceneService;
            _crossSceneView = crossSceneView;
        }
        
        public void Start()
        {
            _service.Print(_view.testString);
            _crossSceneService.Print($"{_crossSceneView.TestString} from cross main menu controller");
            _publisher.Publish($"{_view.testString} from published scene view");
            //_publisherint.Publish(55);
        }
    }
}