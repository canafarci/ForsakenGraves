using VContainer.Unity;

namespace ForsakenGraves.Test
{
    public class CrossScenePresenter : IStartable
    {
        private readonly CrossSceneView _view;
        private readonly CrossSceneService _service;

        public CrossScenePresenter(CrossSceneView view, CrossSceneService service)
        {
            _view = view;
            _service = service;
        }
        
        public void Start()
        {
            _service.Print(_view.TestString);
        }
    }
}