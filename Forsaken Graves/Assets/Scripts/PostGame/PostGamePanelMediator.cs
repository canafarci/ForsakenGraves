using System;
using VContainer.Unity;

namespace ForsakenGraves.PostGame
{
    public class PostGamePanelMediator : IInitializable, IDisposable
    {
        private readonly PostGamePanelView _view;

        public event Action OnReplayButtonClicked;
        public event Action OnMainMenuButtonClicked;

        public PostGamePanelMediator(PostGamePanelView view)
        {
            _view = view;
        }

        public void Initialize()
        {
            _view.ReplayButton.onClick.AddListener(() => OnReplayButtonClicked?.Invoke());
            _view.MainMenuButton.onClick.AddListener(() => OnMainMenuButtonClicked?.Invoke());
        }

        public void Dispose()
        {
            _view.ReplayButton.onClick.RemoveAllListeners();
            _view.MainMenuButton.onClick.RemoveAllListeners();
        }

        public void DisableReplayButton()
        {
            _view.ReplayButton.interactable = false;
        }
    }
}