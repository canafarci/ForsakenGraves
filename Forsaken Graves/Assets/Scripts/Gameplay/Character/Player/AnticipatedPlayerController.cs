using System;
using System.Collections.Generic;
using ForsakenGraves.Gameplay.Data;
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
        [Inject] private NetworkManager _networkManager;
        [Inject] private InputPoller _inputPoller;
        [Inject] private PlayerConfig _playerConfig;
        [Inject] private CharacterController _characterController;
        [Inject] private CapsuleCollider _capsuleCollider;

        private const int BUFFER_SIZE = 1024;
        private int _networkTickRate;
        private NetworkTimer _networkTimer;
        private float _tickTime;

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
            _networkTickRate = (int)_networkManager.NetworkTickSystem.TickRate;
            _networkTimer = new NetworkTimer(_networkTickRate);
            _reconciliationTimer = new CountdownTimer(RECONCILIATION_COOLDOWN_TIME);
            _tickTime = 1f / _networkTickRate;
        }
#endregion

#region Mono Methods
        private void Update()
        {
            _networkTimer.Tick(Time.deltaTime);
            _reconciliationTimer.Tick(Time.deltaTime);
        }

private void FixedUpdate()
        {
            if (!IsSpawned) return;

            while (_networkTimer.ShouldTick())
            {
                HandleMovement();
                HandleRotation();
            }
        }
#endregion

#region Movement
        private void HandleMovement()
        {
            HandleClientTickForMovement();
            HandleServerTickForMovement();
        }
        
    #region Client
        private void HandleClientTickForMovement()
        {
            if (!IsOwner) return;
            
            int currentTick = _networkTimer.CurrentTick;
            int bufferIndex = currentTick % BUFFER_SIZE;

            Vector3 movementInput = _inputPoller.GetMovementInput();
            
            InputPayload inputPayload = new InputPayload()
                                        {
                                            Tick = currentTick,
                                            InputVector = movementInput,
                                            YRotation = transform.eulerAngles.y
                                        };
                
            _clientInputBuffer.Add(inputPayload, bufferIndex);
            SendMovementToServerRpc(inputPayload);

            StatePayload statePayload = ProcessMovement(inputPayload);
            _clientStateBuffer.Add(statePayload, bufferIndex);

            HandleServerReconciliation();
            
#if UNITY_EDITOR            
            //here to test reconciliation
            if (Input.GetKeyDown(KeyCode.Space))
                _characterController.Move(transform.forward * 10f);
#endif            
        }

        private void HandleServerReconciliation()
        {
            if (!ShouldReconcile()) return;

            int bufferIndex = _lastServerState.Tick % BUFFER_SIZE; //not enough data to reconcile
            
            if (bufferIndex - 1 < 0 ) return;

            StatePayload rewindState = IsHost ? _serverStateBuffer.Get(bufferIndex - 1) : _lastServerState;
            StatePayload clientState = IsHost ? _clientStateBuffer.Get(bufferIndex - 1) : _clientStateBuffer.Get(bufferIndex);
            
            float positionError = Vector3.Distance(rewindState.Position, clientState.Position);
            
            if (positionError > RECONCILIATION_POSITION_TRESHOLD)
            {
                ReconcileState(rewindState);
                _reconciliationTimer.Start();
            }

            _lastProcessedServerState = rewindState;
        }

        private void ReconcileState(StatePayload rewindState)
        {
            transform.position = rewindState.Position;
            
            if (!rewindState.Equals(_lastServerState)) return;
            
            _clientStateBuffer.Add(rewindState, rewindState.Tick);
            
            //replay all input from the rewind state to the current state
            int tickToReplay = _lastServerState.Tick;
            while (tickToReplay < _networkTimer.CurrentTick)
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
        private void Move(Vector3 movementInput)
        {
            Vector3 moveInputRelativeToLook = Quaternion.Euler(0, transform.eulerAngles.y, 0) * movementInput;
            _characterController.Move(moveInputRelativeToLook * (_playerConfig.MovementSpeed * _tickTime));
        }
        
        private StatePayload ProcessMovement(InputPayload inputPayload)
        {
            //set y rotation for reconciliation, no effect on local client while regular movement
            if (!Mathf.Approximately(transform.eulerAngles.y, inputPayload.YRotation))
            {
                Quaternion rotation = Quaternion.Euler(0f, inputPayload.YRotation, 0f);
                transform.rotation = rotation;
            }
            
            Move(inputPayload.InputVector);
            
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

            float rotationInput = _inputPoller.GetRotationXInput();
            Rotate(rotationInput);
        }
        
        #endregion

        #region Rotation Methods
            private void Rotate(float rotationInput)
            {
                transform.Rotate(Vector3.up, rotationInput * _playerConfig.RotationSpeed * _tickTime);
            }
       #endregion
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
        }
#endregion
    }
}