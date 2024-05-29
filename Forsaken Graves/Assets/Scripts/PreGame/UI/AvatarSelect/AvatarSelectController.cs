using System;
using ForsakenGraves.PreGame.AvatarSelect;
using ForsakenGraves.PreGame.Data;
using VContainer.Unity;

namespace ForsakenGraves.PreGame.UI.AvatarSelect
{
    public class AvatarSelectController : IInitializable, IDisposable
    {
        private readonly AvatarSelectMediator _mediator;
        private readonly AvatarSelectModel _model;
        private readonly PlayerAvatarsSO _playerAvatarsSO;
        private readonly AvatarDisplayService _avatarDisplayService;
        private readonly PreGameNetwork _preGameNetwork;

        public AvatarSelectController(AvatarSelectMediator mediator,
                                      AvatarSelectModel model,
                                      PlayerAvatarsSO playerAvatarsSO,
                                      PreGameNetwork preGameNetwork)
        {
            _mediator = mediator;
            _model = model;
            _playerAvatarsSO = playerAvatarsSO;
            _preGameNetwork = preGameNetwork;
        }

        public void Initialize()
        {
            _mediator.OnAvatarChangeButtonClicked += AvatarChangeButtonClickedHandler;
        }

        private void AvatarChangeButtonClickedHandler()
        {
            if (_preGameNetwork.IsLobbyLocked.Value) return;
            
            int currentIndex = _model.AvatarIndex;
            int avatarsLength = _playerAvatarsSO.PlayerAvatars.Count;

            int nextIndex = currentIndex + 1 == avatarsLength ? 0 : currentIndex + 1;
            
            _model.ChangeAvatarIndex(nextIndex);
            _preGameNetwork.ChangeAvatarServerRpc(nextIndex);
        }

        public void Dispose()
        {
            _mediator.OnAvatarChangeButtonClicked -= AvatarChangeButtonClickedHandler;
        }
    }
}