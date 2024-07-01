using ForsakenGraves.Gameplay.Data;
using ForsakenGraves.Gameplay.Inputs;
using Unity.Cinemachine;
using Unity.Netcode;
using UnityEngine;
using VContainer;

namespace ForsakenGraves.Gameplay.Character.Player
{
    public class LocalCameraVerticalRotate : NetworkBehaviour
    {
        [Inject] private InputPoller _inputPoller;
        [Inject] private PlayerConfig _playerConfig;
        [Inject] private PlayerCharacterGraphicsSpawner _graphicsSpawner;
        
        private Transform _cameraTransform;
        
        private float _xRotation;
        
        private void Awake()
        {
            _graphicsSpawner.OnAvatarSpawned += AvatarSpawnedHandler;
        }

        private void AvatarSpawnedHandler()
        {
            _cameraTransform = GetComponentInChildren<CinemachineCamera>().transform;
        }

        public override void OnNetworkSpawn()
        {
            if (!IsOwner)
                enabled = false;
        }

        private void Update()
        {
            if (_cameraTransform == null) return;
            
            float mouseYRotation = _inputPoller.GetRotationYInput() * Time.deltaTime * _playerConfig.RotationSpeed;
            if (Mathf.Approximately(0f, mouseYRotation)) return;
            
            _xRotation += mouseYRotation;
            
            _xRotation = Mathf.Clamp(_xRotation, _playerConfig.CameraMinXRotation, _playerConfig.CameraMaxXRotation);

            _cameraTransform.localRotation = Quaternion.Euler(_xRotation, 0, 0);
        }
        
        public override void OnNetworkDespawn()
        {
            _graphicsSpawner.OnAvatarSpawned -= AvatarSpawnedHandler;
        }
    }
}