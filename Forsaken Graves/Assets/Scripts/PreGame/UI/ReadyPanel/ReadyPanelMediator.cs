using System;
using VContainer.Unity;

namespace ForsakenGraves.PreGame.UI.ReadyPanel
{
    public class ReadyPanelMediator : IInitializable, IDisposable
    {
        private readonly ReadyPanelView _view;
        private readonly PreGameNetwork _preGameNetwork;
        public event Action ReadyButtonClicked;

        public ReadyPanelMediator(ReadyPanelView view, PreGameNetwork preGameNetwork)
        {
            _view = view;
            _preGameNetwork = preGameNetwork;
        }
        
        public void Initialize()
        {
            _view.ReadyButton.onClick.AddListener(OnReadyButtonClicked);
        }

        private void OnReadyButtonClicked()
        {
            _view.ReadyButton.interactable = false;
            ReadyButtonClicked?.Invoke();
        }

        public void UpdateReadyButton(bool isReady)
        {
            _view.ReadyButton.interactable = true;

            if (isReady)
            {
                _view.Text.SetText("Waiting for Other Players...");
            }
            else
            {
                _view.Text.SetText("Ready");
            }
        }
        
        public void Dispose()
        {
            _view.ReadyButton.onClick.RemoveAllListeners();
        }
    }
}