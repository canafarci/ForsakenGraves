using System;
using ForsakenGraves.Gameplay.Data;
using ForsakenGraves.Identifiers;
using ForsakenGraves.Infrastructure.Data;
using Unity.Netcode;
using Unity.Netcode.Components;
using UnityEngine;

namespace ForsakenGraves.Gameplay.Character
{
    public class AnticipatedPlayerMove : NetworkBehaviour
    {
        [SerializeField] private AnticipatedNetworkTransform _anticipatedNetworkTransform;
        [SerializeField] private InputPoller _inputPoller;
        [SerializeField] private CharacterController _characterController;
        [SerializeField] private CapsuleCollider _capsuleCollider;
        [SerializeField] private PlayerConfig _playerConfig;
        
        private FrameHistory<Vector3> _positionHistory = new();
        
        //safeguard to prevent sending updates faster than network update rate
        private float _inputSendRate = 0f;
        private float _lastInputSentTime;
        
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
        
        private void MoveAndSendRpc(InputFlags inputs)
        {
            if (ApplyMovement(inputs, Time.deltaTime)) return;
            if (IsHost) return;
            
            if (CanSendInput())
                ServerMoveRpc(transform.position);
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

        private bool ApplyMovement(InputFlags inputs, float deltaTime)
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

            if (direction == Vector3.zero) return true;
            
            _characterController.Move(direction * (_playerConfig.MovementSpeed * deltaTime));
            _anticipatedNetworkTransform.AnticipateMove(transform.position);
            return false;
        }
        
        private void OnNetworkTick()
        {
            double localTime = NetworkManager.LocalTime.Time;
            _positionHistory.Add(localTime, transform.position);
        }
        
        public override void OnReanticipate(double lastRoundTripTime)
        {
            if (!IsOwner) return;
            
            AnticipatedNetworkTransform.TransformState previousState = _anticipatedNetworkTransform.PreviousAnticipatedState;

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

            Vector3 anticipationError = nearestPositionFrame.Item - previousState.Position;
            _characterController.Move(anticipationError);

            Debug.Log($"time {Time.frameCount}, error: {anticipationError}, timeDelta : {timeDelta}");          
            _positionHistory.RemoveBefore(authorityTime);
        }

        [Rpc(SendTo.Server)]
        private void ServerMoveRpc(Vector3 position)
        {
            _anticipatedNetworkTransform.AnticipateMove(position);
        }

        private void Update()
        {
            if (!IsOwner) return;

            InputFlags input =  _inputPoller.GetMovementInput();
            MoveAndSendRpc(input);
        }

        public override void OnNetworkDespawn()
        {
             NetworkManager.NetworkTickSystem.Tick -= OnNetworkTick;
        }
    }
}