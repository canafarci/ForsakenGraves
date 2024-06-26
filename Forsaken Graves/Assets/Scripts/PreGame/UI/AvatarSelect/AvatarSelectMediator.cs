using System;
using VContainer.Unity;

namespace ForsakenGraves.PreGame.UI.AvatarSelect
{
    public class AvatarSelectMediator : IInitializable, IDisposable
    {
        private readonly AvatarSelectView _view;
        public event Action OnAvatarChangeButtonClicked;

        public AvatarSelectMediator(AvatarSelectView view)
        {
            _view = view;
        }
        
        public void Initialize()
        {
            _view.ChangeAvatarButton.onClick.AddListener(OnChangeAvatarButtonClicked);
        }

        private void OnChangeAvatarButtonClicked()
        {
            _view.ChangeAvatarButton.interactable = false;
            OnAvatarChangeButtonClicked?.Invoke();
        }

        public void EnableButton()
        {
            _view.ChangeAvatarButton.interactable = true;
        }

        public void Dispose()
        {
            _view.ChangeAvatarButton.onClick.RemoveAllListeners();
        }
    }
}