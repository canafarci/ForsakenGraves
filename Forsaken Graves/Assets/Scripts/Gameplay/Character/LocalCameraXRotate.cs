using System;
using ForsakenGraves.Gameplay.Data;
using Unity.Netcode;
using UnityEngine;

namespace ForsakenGraves.Gameplay.Character
{
    public class LocalCameraXRotate : NetworkBehaviour
    {
        [SerializeField] private InputPoller _inputPoller; 
        [SerializeField] private PlayerConfig _playerConfig;
        [SerializeField] private Transform _cameraTransform;

        public override void OnNetworkSpawn()
        {
            if (!IsOwner)
                enabled = false;
        }

        private void Update()
        {
            float mouseYRotation = _inputPoller.GetRotationYInput();
            if (Mathf.Approximately(0f, mouseYRotation)) return;
            
            _cameraTransform.Rotate(Vector3.left, mouseYRotation * _playerConfig.RotationSpeed * Time.deltaTime);
            float clampedXRotationValue = _cameraTransform.rotation.eulerAngles.x;

            if (clampedXRotationValue is >= 0 and < 180)
                clampedXRotationValue = Mathf.Clamp(clampedXRotationValue, 0 , 45);
            else if (clampedXRotationValue is > 180 and <= 360)
                clampedXRotationValue = Mathf.Clamp(clampedXRotationValue, 270 , 360);

            Vector3 clampedRotation = _cameraTransform.rotation.eulerAngles;
            clampedRotation.x = clampedXRotationValue;

            _cameraTransform.rotation = Quaternion.Euler(clampedRotation);
        }
    }
}