using ForsakenGraves.Gameplay.Data;
using ForsakenGraves.Gameplay.Inputs;
using Unity.Netcode;
using UnityEngine;
using VContainer;

namespace ForsakenGraves.Gameplay.Character.Player
{
    public class LocalCameraVerticalRotate : NetworkBehaviour
    {
        [Inject] private InputPoller _inputPoller;
        [Inject] private PlayerConfig _playerConfig;
        
        [SerializeField] private Transform _cameraTransform;
        
        private float _xRotation;

        
        public override void OnNetworkSpawn()
        {
            if (!IsOwner)
                enabled = false;
        }

        private void Update()
        {
            float mouseYRotation = _inputPoller.GetRotationYInput();
            if (Mathf.Approximately(0f, mouseYRotation)) return;
            
            _xRotation += mouseYRotation;
            
            _xRotation = Mathf.Clamp(_xRotation, _playerConfig.CameraMinXRotation, _playerConfig.CameraMaxXRotation);

            _cameraTransform.localRotation = Quaternion.Euler(_xRotation, 0, 0);
        }
    }
}