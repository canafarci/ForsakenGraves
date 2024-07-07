using ForsakenGraves.Gameplay.Cameras;
using Unity.Netcode;
using VContainer;

namespace ForsakenGraves.Gameplay.Character.Player
{
    public class ClientCameraController : NetworkBehaviour
    {
        [Inject] private PlayerCharacterGraphicsSpawner _graphicsSpawner;
        [Inject] private CameraController _cameraController;
        
        private void Awake()
        {
            _graphicsSpawner.OnAvatarSpawned += AvatarSpawnedHandler;
        }

        private void AvatarSpawnedHandler()
        {
            if (IsOwner)
            {
                _cameraController.SetCameraTargetReference(GetComponentInChildren<CameraTargetReference>());
                _cameraController.SetHandsTransform(GetComponentInChildren<HandsSpawnTransform>());
                _cameraController.SetGameplayCamera();
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