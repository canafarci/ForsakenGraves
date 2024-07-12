using System;
using ForsakenGraves.Gameplay.Cameras;
using ForsakenGraves.Gameplay.Data;
using ForsakenGraves.Visuals.Animations;
using Unity.Netcode;
using VContainer;

namespace ForsakenGraves.Gameplay.Character.Player
{
    public class ClientCameraController : NetworkBehaviour
    {
        [Inject] private PlayerCharacterGraphicsSpawner _graphicsSpawner;
        [Inject] private CameraController _cameraController;
        [Inject] private PlayerConfig _playerConfig;
        [Inject] private AnticipatedPlayerController _anticipatedPlayerController;
        
        private CameraTargetReference _targetReference;
        private HandsFacade _handsFacade;

        private void Awake()
        {
            _graphicsSpawner.OnAvatarSpawned += AvatarSpawnedHandler;
        }

        private void AvatarSpawnedHandler()
        {
            if (IsOwner)
            {
                _targetReference = GetComponentInChildren<CameraTargetReference>();
                _cameraController.SetCameraTargetReference(_targetReference);
                _cameraController.SetGameplayCameraTargets();

                _handsFacade = GetComponentInChildren<HandsFacade>();
                _handsFacade.InitializeHandsFollow(_targetReference.HandsFollowTransform, _playerConfig, _anticipatedPlayerController);
            }
            else
            {
                _cameraController.Dispose();
            }
        }

        public override void OnNetworkDespawn()
        {
            _graphicsSpawner.OnAvatarSpawned -= AvatarSpawnedHandler;
        }

    }
}