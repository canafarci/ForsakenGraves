using System;
using ForsakenGraves.Identifiers;
using ForsakenGraves.Infrastructure.Data;
using Unity.Netcode;
using Unity.Netcode.Components;
using UnityEngine;

namespace ForsakenGraves.Gameplay.Character
{
    public class ClientPlayerMove : NetworkBehaviour
    {
        [SerializeField] private AnticipatedNetworkTransform _anticipatedNetworkTransform;
        [SerializeField] private InputPoller _inputPoller;
        [SerializeField] private CharacterController _characterController;
        [SerializeField] private CapsuleCollider _capsuleCollider;
        
        private FrameHistory<Vector3> _positionHistory = new();
        
        private float _smoothTime = 0.1f;
        private float _smoothDistance = 3f;

        public override void OnNetworkSpawn()
        {
            _anticipatedNetworkTransform.StaleDataHandling = StaleDataHandling.Ignore;

            if (IsHost)
            {
                EnableForAuthority();
            }
            else if (IsServer)
            {
                _capsuleCollider.enabled = false;
                _characterController.enabled = true;
                enabled = false;
            }
            else if (!IsOwner)
            {
                _capsuleCollider.enabled = true;
                _characterController.enabled = false;
                enabled = false;
            }
            else //is client
            {
                EnableForAuthority();
            }
        }

        private void EnableForAuthority()
        {
            _characterController.enabled = true;
            _capsuleCollider.enabled = false;
            
            NetworkManager.NetworkTickSystem.Tick += OnNetworkTick;
        }
        
        private void MoveAndSendRpc(InputFlags inputs)
        {
            if (ApplyMovement(inputs, Time.deltaTime)) return;
            if (IsHost) return;
            
            ServerMoveRpc(transform.position, Time.deltaTime);
        }

        private bool ApplyMovement(InputFlags inputs, float deltaTime)
        {
            Vector3 direction = Vector3.zero;
            
            if ((inputs & InputFlags.Up) != 0)
            {
                direction += transform.forward;
            }

            if ((inputs & InputFlags.Down) != 0)
            {
                direction -= transform.forward;
            }

            if ((inputs & InputFlags.Left) != 0)
            {
                direction -= transform.right;
            }

            if ((inputs & InputFlags.Right) != 0)
            {
                direction += transform.right;
            }
            
            if (direction == Vector3.zero) return true;
            
            _characterController.Move(direction * (5f * deltaTime));
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
            
            _positionHistory.RemoveBefore(authorityTime);
        }

        [Rpc(SendTo.Server)]
        private void ServerMoveRpc(Vector3 position, float dt)
        {
            _anticipatedNetworkTransform.AnticipateMove(position);
        }

        private void Update()
        {
            if (!IsOwner) return;

            InputFlags input =  _inputPoller.GetInput();
            MoveAndSendRpc(input);
            Debug.Log($"Time: {Time.frameCount}, Pos : {transform.position}");

        }

        public override void OnNetworkDespawn()
        {
             NetworkManager.NetworkTickSystem.Tick -= OnNetworkTick;
        }
    }
}