using System;
using ForsakenGraves.Gameplay.Data;
using Unity.Netcode;
using Unity.Netcode.Components;
using UnityEngine;

namespace ForsakenGraves.Gameplay.Character
{
    //handles character's Y rotation, which is server authoritative with client side anticipation
    public class AnticipatedPlayerYRotate : NetworkBehaviour
    {
        [SerializeField] private InputPoller _inputPoller; 
        [SerializeField] private PlayerConfig _playerConfig;
        [SerializeField] private AnticipatedNetworkTransform _anticipatedNetworkTransform;
        
        //safeguard to prevent sending updates faster than network update rate
        private float _inputSendRate = 0f;
        private float _lastInputSentTime;
        
        public override void OnNetworkSpawn()
        {
            if (!IsOwner) return;
            
            //set max input send rate
            uint tickRate = NetworkManager.NetworkTickSystem.TickRate;
            _inputSendRate = 1f / tickRate;
        }

        private void Update()
        {
            if (!IsOwner) return;
            
            float mouseXRotation = _inputPoller.GetRotationInput();
            
            if (ApplyRotation(mouseXRotation)) return;

            if (CanSendInput() && !IsHost)
                ServerRotateRpc(transform.rotation);
        }

        private bool ApplyRotation(float mouseXRotation)
        {
            if (Mathf.Approximately(0f, mouseXRotation)) return true;
            
            transform.Rotate(Vector3.up, mouseXRotation * _playerConfig.RotationSpeed);
            _anticipatedNetworkTransform.AnticipateRotate(transform.rotation);
            
            return false;
        }
        
        [Rpc(SendTo.Server)]
        private void ServerRotateRpc(Quaternion rotation)
        {
            _anticipatedNetworkTransform.AnticipateRotate(rotation);
        }
        
        private bool CanSendInput()
        {
            if (_lastInputSentTime + _inputSendRate < Time.time)
            {
                _lastInputSentTime = Time.time;
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}