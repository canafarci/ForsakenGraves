using System;
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
        
        private FrameHistory<Vector3> _positionHistory = new();
        
        //safeguard to prevent sending updates faster than network update rate
        private float _inputSendRate = 0f;
        private float _lastInputSentTime;

        private const float SMOOTH_TIME = 0.1f;
        private const float TELEPORT_DISTANCE_SQR = 3f;
        private const float SMOOTH_DISTANCE_SQR = 1.6f;
        
        public override void OnNetworkSpawn()
        {
            _anticipatedNetworkTransform.StaleDataHandling = StaleDataHandling.Ignore;
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
            
            NetworkManager.NetworkTickSystem.Tick += OnNetworkTick;
            
            //set max input send rate
            uint tickRate = NetworkManager.NetworkTickSystem.TickRate;
            _inputSendRate = 1f / tickRate;
        }
        
        private void Update()
        {
            if (!IsOwner) return;

            InputFlags input =  _inputPoller.GetMovementInput();
            MoveAndSendRpc(input);
        }
        
        private void MoveAndSendRpc(InputFlags inputs)
        {
            Vector3 direction = GetDirection(inputs);

            Move(direction);

            if (IsHost) return;
            
            if (CanSendInput())
                ServerMoveRpc(direction);
        }

        private void Move(Vector3 direction)
        {
            _characterController.Move(direction * (_playerConfig.MovementSpeed * Time.deltaTime));
            
            if (IsHost)
            {
                if (CanSendInput())
                {
                    _anticipatedNetworkTransform.AnticipateMove(transform.position);
                }
            }
            else
            {
                _anticipatedNetworkTransform.AnticipateMove(transform.position);
            }
            
            //Debug.Log($"time {Time.frameCount}, calledFromIsServer: {IsServer},  calledFromIsOwner: {IsOwner}");          
        }

        [Rpc(SendTo.Server)]
        private void ServerMoveRpc(Vector3 direction)
        {
            Move(direction);
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
        
        private void OnNetworkTick()
        {
            double localTime = NetworkManager.LocalTime.Time;
            _positionHistory.Add(localTime, transform.position);
        }
        
        public override void OnReanticipate(double lastRoundTripTime)
        {
            AnticipatedNetworkTransform.TransformState previousState = _anticipatedNetworkTransform.PreviousAnticipatedState;
            
            if (!IsOwner)
            {
                _anticipatedNetworkTransform.Smooth(previousState,
                                                    _anticipatedNetworkTransform.AuthoritativeState,
                                                    SMOOTH_TIME);
                return;
            }


            double localTime = NetworkManager.LocalTime.Time;
            double authorityTime = localTime - lastRoundTripTime;

            double timeDelta = Mathf.Infinity;
            FrameHistory<Vector3>.ItemFrameData nearestPositionFrame = default;

            foreach (FrameHistory<Vector3>.ItemFrameData positionFrame in _positionHistory.GetHistory())
            {
                double frameTime = positionFrame.Time;
                double localTimeDelta = Math.Abs(frameTime - authorityTime);

                if (localTimeDelta < timeDelta)
                {
                    nearestPositionFrame = positionFrame;
                    timeDelta = localTimeDelta;
                }
            }
            
            _positionHistory.RemoveBefore(authorityTime);

            Vector3 anticipationError = nearestPositionFrame.Item - previousState.Position;

            float errorSquared = anticipationError.sqrMagnitude;
            
            if (errorSquared < SMOOTH_DISTANCE_SQR)
            {
                //error rate is small, just add movement 
                _characterController.Move(anticipationError);
            }
            else if (errorSquared < TELEPORT_DISTANCE_SQR)
            {
                //error rate is negligible, smooth position
                _anticipatedNetworkTransform.Smooth(previousState,
                                                    _anticipatedNetworkTransform.AuthoritativeState,
                                                    SMOOTH_TIME);
            }
            else
            {
                //error rate is too high, teleport to authoritative position
                _anticipatedNetworkTransform.AnticipateMove(_anticipatedNetworkTransform.AuthoritativeState.Position);
            }
            
            Debug.Log($"time {Time.frameCount}, error: {anticipationError}, timeDelta : {timeDelta}");          
        }

        public override void OnNetworkDespawn()
        {
             NetworkManager.NetworkTickSystem.Tick -= OnNetworkTick;
        }
    }
}