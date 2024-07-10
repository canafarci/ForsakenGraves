using System;
using System.Collections.Generic;
using ForsakenGraves.Gameplay.Cameras;
using ForsakenGraves.Gameplay.Data;
using ForsakenGraves.Gameplay.Inputs;
using ForsakenGraves.Identifiers;
using ForsakenGraves.Infrastructure.Networking;
using ForsakenGraves.Infrastructure.Networking.Data;
using ForsakenGraves.Infrastructure.Networking.DataStructures;
using ForsakenGraves.Utils.Utilities;
using Unity.Netcode;
using UnityEngine;
using VContainer;

namespace ForsakenGraves.Gameplay.Character.Player
{
    public class AnticipatedPlayerController : NetworkBehaviour
    {
        [Inject] private InputPoller _inputPoller;
        [Inject] private PlayerConfig _playerConfig;
        [Inject] private CharacterController _characterController;
        [Inject] private CapsuleCollider _capsuleCollider;
        [Inject] private PlayerCharacterGraphicsSpawner _graphicsSpawner;
        
        //netcode data
        private const int BUFFER_SIZE = 1024;
        
        //rotation
        private float _cameraXRotation;
        private Transform _cameraTransform;
        
        //jumping
        private float _yVelocity = 0f;

        //client data
        private CircularBuffer<InputPayload> _clientInputBuffer = new(BUFFER_SIZE);
        private CircularBuffer<StatePayload> _clientStateBuffer = new(BUFFER_SIZE);

        //last state received from server
        private StatePayload _lastServerState;

        //last server state processed and reconciled with the server
        private StatePayload _lastProcessedServerState;

        //server data
        private CircularBuffer<StatePayload> _serverStateBuffer = new(BUFFER_SIZE);
        private Queue<InputPayload> _serverInputQueue = new();
        
        //reconciliation
        private CountdownTimer _reconciliationTimer;
        
        private const float RECONCILIATION_POSITION_TRESHOLD = 0.05f;
        private const float RECONCILIATION_COOLDOWN_TIME = 1f;
        
        public override void OnNetworkSpawn()
        {
            ChangeComponentActivationRelativeToAuthority();
        }

#region Initialization
        private void Awake()
        {
            _reconciliationTimer = new CountdownTimer(RECONCILIATION_COOLDOWN_TIME);
            _graphicsSpawner.OnAvatarSpawned += AvatarSpawnedHandler;
        }

        #endregion

#region Mono Methods
        private void Update()
        {
            _reconciliationTimer.Tick(Time.deltaTime);
            //this is client authoritative, but updates in network loop to prevent desync and animation issues
            HandleRotation();
        }

        private void NetworkTick(int currentTick)
        {
            if (!IsSpawned) return;

            Debug.Log("called");
            
            HandleClientTickForMovement(currentTick);
            HandleServerTickForMovement();
        }
#endregion

#region Movement
    #region Client
        private void HandleClientTickForMovement(int currentTick)
        {
            if (!IsOwner) return;
            
            int bufferIndex = currentTick % BUFFER_SIZE;

            InputFlags movementInput = _inputPoller.GetMovementInput();
            
            InputPayload inputPayload = new InputPayload()
                                        {
                                            Tick = currentTick,
                                            Input = movementInput,
                                            YRotation = transform.eulerAngles.y
                                        };
                
            _clientInputBuffer.Add(inputPayload, bufferIndex);
            SendMovementToServerRpc(inputPayload);

            if (!IsServer) //to avoid duplicate calls to the move method on host
            {
                StatePayload statePayload = ProcessMovement(inputPayload);
                _clientStateBuffer.Add(statePayload, bufferIndex);
            }

            HandleServerReconciliation(currentTick);
            
#if UNITY_EDITOR            
            //here to test reconciliation
            if (Input.GetKeyDown(KeyCode.G))
                _characterController.Move(transform.forward * 10f);
#endif            
        }

        private void HandleServerReconciliation(int currentTick)
        {
            if (!ShouldReconcile()) return;

            int bufferIndex = _lastServerState.Tick % BUFFER_SIZE; //not enough data to reconcile
            
            if (bufferIndex - 1 < 0 ) return;

            StatePayload rewindState = IsHost ? _serverStateBuffer.Get(bufferIndex - 1) : _lastServerState;
            StatePayload clientState = IsHost ? _clientStateBuffer.Get(bufferIndex - 1) : _clientStateBuffer.Get(bufferIndex);
            
            float positionError = Vector3.Distance(rewindState.Position, clientState.Position);
            
            if (positionError > RECONCILIATION_POSITION_TRESHOLD)
            {
                ReconcileState(rewindState, currentTick);
                _reconciliationTimer.Start();
            }

            _lastProcessedServerState = rewindState;
        }

        private void ReconcileState(StatePayload rewindState, int currentTick)
        {
            transform.position = rewindState.Position;
            
            if (!rewindState.Equals(_lastServerState)) return;
            
            _clientStateBuffer.Add(rewindState, rewindState.Tick);
            
            //replay all input from the rewind state to the current state
            int tickToReplay = _lastServerState.Tick;
            while (tickToReplay < currentTick)
            {
                Physics.SyncTransforms();
                int bufferIndex = tickToReplay % BUFFER_SIZE;
                InputPayload inputPayload = _clientInputBuffer.Get(bufferIndex);
                StatePayload statePayload = ProcessMovement(inputPayload);
                _clientStateBuffer.Add(statePayload, bufferIndex % BUFFER_SIZE);
                tickToReplay++;
            }
        }

        private bool ShouldReconcile()
        {
            bool isNewServerState = !_lastServerState.Equals(default);
            bool isLastStateUndefinedOrDefault = _lastProcessedServerState.Equals(default) ||
                                                 !_lastProcessedServerState.Equals(_lastServerState);

            return isNewServerState && isLastStateUndefinedOrDefault && !_reconciliationTimer.IsRunning;
        }
    #endregion

    #region Server
        private void HandleServerTickForMovement()
        {
            if (!IsServer) return;
            
            int bufferIndex = -1; //safeguard to throw exception

            while (_serverInputQueue.Count > 0)
            {
                InputPayload inputPayload = _serverInputQueue.Dequeue();
                bufferIndex = inputPayload.Tick % BUFFER_SIZE;
                
                StatePayload statePayload = ProcessMovement(inputPayload);
                _serverStateBuffer.Add(statePayload, bufferIndex);
            }
            
            if (bufferIndex == -1) return;
            StatePayload payload = _serverStateBuffer.Get(bufferIndex);
            SendToClientRpc(payload);
        }

        [Rpc(SendTo.ClientsAndHost)]
        private void SendToClientRpc(StatePayload statePayload)
        {
            if (!IsOwner) return;
            _lastServerState = statePayload;
        }

        [Rpc(SendTo.Server)]
        private void SendMovementToServerRpc(InputPayload inputPayload)
        {
            _serverInputQueue.Enqueue(inputPayload);
        }
    #endregion
    
    #region Movement Methods
        private void Move(InputFlags movementInput)
        {
            Vector3 movementDirection = GetMovementVector(movementInput);
            Vector3 moveInputRelativeToLook = Quaternion.Euler(0, transform.eulerAngles.y, 0) * movementDirection;
            
            UpdateVerticalVelocity(movementInput);
            moveInputRelativeToLook.y = _yVelocity;
            
            bool isGrounded = _characterController.isGrounded;
            if (isGrounded && _yVelocity < 0)
            {
                _yVelocity = 0f;
            }
            
            _characterController.Move(moveInputRelativeToLook * (_playerConfig.MovementSpeed * NetworkTicker.TickRate));
        }

        private Vector3 GetMovementVector(InputFlags movementInput)
        {
            Vector3 movementVector = Vector3.zero;
            
            if ((movementInput & InputFlags.Forward) != 0) 
                movementVector += Vector3.forward;

            if ((movementInput & InputFlags.Back) != 0)  
                movementVector -= Vector3.forward;

            if ((movementInput & InputFlags.Left) != 0) 
                movementVector -= Vector3.right;

            if ((movementInput & InputFlags.Right) != 0) 
                movementVector += Vector3.right;

            return movementVector;
        }

        private StatePayload ProcessMovement(InputPayload inputPayload)
        {
            //set y rotation for reconciliation, no effect on local client while regular movement
            if (!Mathf.Approximately(transform.eulerAngles.y, inputPayload.YRotation))
            {
                Quaternion rotation = Quaternion.Euler(0f, inputPayload.YRotation, 0f);
                transform.rotation = rotation;
            }
            
            Move(inputPayload.Input);
            
            return new StatePayload()
                   {
                       Position = transform.position,
                       Tick = inputPayload.Tick,
                       YRotation = transform.eulerAngles.y
                   };
        }
    #endregion
#endregion
        
#region Rotation 
    #region Client
        // rotation is client authoritative
        private void HandleRotation()
        {
            if (!IsOwner)  return;

            Vector2 rotationInput = _inputPoller.GetRotationInput();
            float horizontalInput = rotationInput.x;
            float verticalInput = rotationInput.y;
            
            RotateHorizontal(horizontalInput);
            RotateCameraVertical(verticalInput);
        }

        #endregion

        #region Rotation Methods
        private void RotateHorizontal(float rotationInput)
        {
            transform.Rotate(Vector3.up, rotationInput * _playerConfig.RotationSpeed * Time.fixedDeltaTime);
        }
        
        private void RotateCameraVertical(float verticalInput)
        {
            if (Mathf.Approximately(0f, verticalInput)) return;
            
            _cameraXRotation += verticalInput * _playerConfig.RotationSpeed * Time.fixedDeltaTime;
            _cameraXRotation = Mathf.Clamp(_cameraXRotation, _playerConfig.CameraMinXRotation, _playerConfig.CameraMaxXRotation);
            _cameraTransform.localRotation = Quaternion.Euler(_cameraXRotation, 0, 0);
        }
        
    #endregion
#endregion

#region Jumping
        private void UpdateVerticalVelocity(InputFlags movementInput)
        {
            bool characterIsGrounded = _characterController.isGrounded;
            if (characterIsGrounded && _yVelocity < 0)
            {
                _yVelocity = 0f;
            }

            if ((movementInput & InputFlags.Jump) != 0 && characterIsGrounded)
            {
                AddVerticalVelocity();
            }
            
            _yVelocity += Physics.gravity.y * Time.deltaTime;

        }

        private void AddVerticalVelocity()
        {
            _yVelocity += Mathf.Sqrt(_playerConfig.JumpHeight * -1.0f * Physics.gravity.y);
        }
#endregion
        
#region Component Activation regarding Ownership
        private void ChangeComponentActivationRelativeToAuthority()
        {
            if (IsServer || IsOwner)
            {
                EnableForAuthority();
            }
            else
            {
                DisableForOtherClient();
            }
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
            
            NetworkTicker.OnNetworkTick += NetworkTick;
            
        }

        private void AvatarSpawnedHandler()
        {
            if (IsOwner)
                _cameraTransform = GetComponentInChildren<CameraTargetReference>().CameraTransform;
        }

        public override void OnDestroy()
        {
            NetworkTicker.OnNetworkTick -= NetworkTick;

            if (IsOwner || IsServer)
            {
                NetworkTicker.OnNetworkTick -= NetworkTick;
            }
            
            _graphicsSpawner.OnAvatarSpawned -= AvatarSpawnedHandler;
            base.OnDestroy();
        }

        #endregion
    }
}