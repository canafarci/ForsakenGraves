using ForsakenGraves.Gameplay.Data;
using ForsakenGraves.Identifiers;
using ForsakenGraves.Infrastructure.Data;
using Unity.Netcode;
using Unity.Netcode.Components;
using UnityEngine;
using VContainer;

namespace ForsakenGraves.Gameplay.Character.Player
{
    public class AnticipatedPlayerMove : NetworkBehaviour
    {
        [Inject] private InputPoller _inputPoller;
        [Inject] private PlayerConfig _playerConfig;
        [Inject] private AnticipatedNetworkTransform _anticipatedNetworkTransform;
        [Inject] private CharacterController _characterController;
        [Inject] private CapsuleCollider _capsuleCollider;
        
        private float _lastInputSentTime;

        private const float NETWORK_TICK_RATE = 30;
        private const float SMOOTH_TIME = 1f / NETWORK_TICK_RATE;
        
        private const float SMOOTH_DISTANCE_SQR = 3f * 3f;
        private const float SMALL_ERROR_SQR = 0.25f * 0.25f;
        
        public override void OnNetworkSpawn()
        {
            _anticipatedNetworkTransform.StaleDataHandling = StaleDataHandling.Reanticipate;
            ChangeComponentActivationRelativeToAuthority();
        }

        private void ChangeComponentActivationRelativeToAuthority()
        {
            if (IsHost)
                EnableForAuthority();
            else if (IsServer)
                DisableForOtherClient();
            else if (!IsOwner)
                DisableForOtherClient();
            else //is client
                EnableForAuthority();
        }

        private void DisableForOtherClient()
        {
            _capsuleCollider.enabled = false;
            _characterController.enabled = true;
            enabled = false;
        }

        private void EnableForAuthority()
        {
            _characterController.enabled = true;
            _capsuleCollider.enabled = false;
        }

        private void Move(InputFlags moveInput)
        {
            Vector3 direction = GetDirection(moveInput);
            
            _characterController.Move(direction * (_playerConfig.MovementSpeed * Time.fixedDeltaTime));
            _anticipatedNetworkTransform.AnticipateMove(transform.position);
            
            Debug.Log(transform.position);
        }
        
        private void Rotate(float mouseXRotation)
        {
            if (Mathf.Approximately(0f, mouseXRotation)) return;
            
            transform.Rotate(Vector3.up, mouseXRotation * _playerConfig.RotationSpeed * Time.fixedDeltaTime);
            _anticipatedNetworkTransform.AnticipateRotate(transform.rotation);
        }

        [Rpc(SendTo.Server)]
        private void PlayerControlServerRpc(InputFlags movementInput, float rotation)
        {
            Move(movementInput);
            //Rotate(rotation);
            if (!IsOwner)
                Debug.Log(transform.position);
            
            //smooth on server side
            AnticipatedNetworkTransform.TransformState currentPosition = _anticipatedNetworkTransform.AnticipatedState;
            _anticipatedNetworkTransform.Smooth(currentPosition, _anticipatedNetworkTransform.AuthoritativeState, SMOOTH_TIME);
        }
        
        public override void OnReanticipate(double lastRoundTripTime)
        {
            if (!_anticipatedNetworkTransform.ShouldReanticipate) return;
            
            //cache previous false anticipated state
            AnticipatedNetworkTransform.TransformState previousState = _anticipatedNetworkTransform.PreviousAnticipatedState;
            
            double authorityTime = NetworkManager.LocalTime.Time - lastRoundTripTime;

            int calledTimes = 0;
            
            //reanticipate based on stored previous input
            foreach (FrameHistory<InputFlags>.ItemFrameData item in _inputPoller.InputHistory.GetHistory())
            {
                if (item.Time <= authorityTime)
                {
                    continue;
                }
                
                Move(item.Item);
                calledTimes++;
            }

            if (Time.timeSinceLevelLoad > 2f)
            {
                Debug.Log("ddd");
            }
                

            Debug.Log(calledTimes);
            
            //remove all input before last server time
            _inputPoller.InputHistory.RemoveBefore(authorityTime);
            
            //handle by smoothing based on error distance
            if (SMOOTH_TIME != 0.0)
            {
                float sqDist = Vector3.SqrMagnitude(previousState.Position - _anticipatedNetworkTransform.AnticipatedState.Position);
                if (sqDist <= SMOOTH_DISTANCE_SQR)
                {
                    // This prevents small amounts of wobble from slight differences.
                    _anticipatedNetworkTransform.AnticipateState(previousState);
                }
                else if (sqDist < SMOOTH_DISTANCE_SQR)
                {
                    //smooth if distance is big
                    _anticipatedNetworkTransform.Smooth(previousState, _anticipatedNetworkTransform.AnticipatedState, SMOOTH_TIME);
                }
                else //error is too big, just teleport
                {
                    _anticipatedNetworkTransform.AnticipateMove(_anticipatedNetworkTransform.AuthoritativeState.Position);
                }
            }
        }
        
        private Vector3 GetDirection(InputFlags inputs)
        {
            Vector3 direction = Vector3.zero;
            
            if ((inputs & InputFlags.Up) != 0) 
                direction += transform.forward;

            if ((inputs & InputFlags.Down) != 0) 
                direction -= transform.forward;

            if ((inputs & InputFlags.Left) != 0) 
                direction -= transform.right;

            if ((inputs & InputFlags.Right) != 0) 
                direction += transform.right;
            
            return direction;
        }
        
        private void FixedUpdate()
        {
            if (!IsOwner || !IsSpawned) return;

            InputFlags movementInput =  _inputPoller.GetMovementInput();
            float mouseXRotationInput = _inputPoller.GetRotationXInput();
            
            if (movementInput == 0) return;
            
            Move(movementInput);
            //Rotate(mouseXRotationInput);

            if (!IsServer)
            {
                PlayerControlServerRpc(movementInput, mouseXRotationInput);
            }
        }
    }
}