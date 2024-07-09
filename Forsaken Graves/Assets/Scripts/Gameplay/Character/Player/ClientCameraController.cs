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
        
        private FollowHands _handsFollow;
        private CameraTargetReference _targetReference;

        private void Awake()
        {
            _graphicsSpawner.OnAvatarSpawned += AvatarSpawnedHandler;
        }

        private void AvatarSpawnedHandler()
        {
            if (IsOwner)
            {
                _targetReference = GetComponentInChildren<CameraTargetReference>();
                // _cameraController.SetCameraTargetReference(_targetReference);
                //
                // _cameraController.SetGameplayCamera();

                _handsFollow = GetComponentInChildren<FollowHands>();
                _handsFollow.Initialize(_targetReference.HandsFollowTransform, _playerConfig);
                
                _handsFollow.transform.SetParent(null);
            }
            else
            {
                // _cameraController.Dispose();
            }
        }

        private void Update()
        {
            if (!IsOwner || !_handsFollow) return;
        }

        public override void OnNetworkDespawn()
        {
            _graphicsSpawner.OnAvatarSpawned -= AvatarSpawnedHandler;
        }

    }
}