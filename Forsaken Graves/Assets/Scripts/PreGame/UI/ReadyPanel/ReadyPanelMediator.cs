using VContainer.Unity;

namespace ForsakenGraves.PreGame.UI.ReadyPanel
{
    public class ReadyPanelMediator : IInitializable
    {
        private readonly ReadyPanelView _view;
        private readonly ClientNetworkedPreGameLogic _clientNetworkedPreGameLogic;

        public ReadyPanelMediator(ReadyPanelView view, ClientNetworkedPreGameLogic clientNetworkedPreGameLogic)
        {
            _view = view;
            _clientNetworkedPreGameLogic = clientNetworkedPreGameLogic;
        }
        
        public void Initialize()
        {
            _view.ReadyButton.onClick.AddListener(OnReadyButtonClicked);
        }

        private void OnReadyButtonClicked()
        {
            _view.ReadyButton.interactable = false;
            _clientNetworkedPreGameLogic.OnReadyClickedServerRpc();
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
    }
}