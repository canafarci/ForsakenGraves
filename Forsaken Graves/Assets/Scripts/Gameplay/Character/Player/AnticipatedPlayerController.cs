using System.Collections.Generic;
using ForsakenGraves.Gameplay.Data;
using ForsakenGraves.Infrastructure.Networking;
using ForsakenGraves.Infrastructure.Networking.Data;
using ForsakenGraves.Infrastructure.Networking.DataStructures;
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
        private uint _networkTickRate;
        private NetworkTimer _networkTimer;
        private float _tickTime;

        //client data
        private CircularBuffer<StatePayload> _clientStateBuffer = new(BUFFER_SIZE);

        private CircularBuffer<InputPayload> _clientInputBuffer = new(BUFFER_SIZE);

        //last state received from server
        private StatePayload _lastServerState;

        //last server state processed and reconciled with the server
        private StatePayload _lastProcessedServerState;

        //server data
        private CircularBuffer<StatePayload> _serverStateBuffer = new(BUFFER_SIZE);
        private Queue<InputPayload> _serverInputQueue = new();

        public override void OnNetworkSpawn()
        {
            ChangeComponentActivationRelativeToAuthority();
        }

#region Initialization
        private void Awake()
        {
            _networkTickRate = _networkManager.NetworkTickSystem.TickRate;
            _networkTimer = new NetworkTimer(_networkTickRate);
            _tickTime = 1f / _networkTickRate;
        }
#endregion

#region Mono Methods
        private void Update()
        {
            _networkTimer.Tick(Time.deltaTime);
            if (!_networkTimer.ShouldTick() || !IsSpawned) return;

            HandleClientTick();
            HandleServerTick();
        }
        #endregion

#region Network State Handling
    #region Client
        private void HandleClientTick()
        {
            int currentTick = _networkTimer.CurrentTick;
            int bufferIndex = currentTick % BUFFER_SIZE;

            Vector3 movementInput = _inputPoller.GetMovementInput();
            Debug.Log(movementInput);
            
            InputPayload inputPayload = new InputPayload()
                                        {
                                            Tick = currentTick,
                                            InputVector = movementInput
                                        };
                
            _clientInputBuffer.Add(inputPayload, bufferIndex);
            SendMovementToServerRpc(inputPayload);

            StatePayload statePayload = ProcessMovement(inputPayload);
            _clientStateBuffer.Add(statePayload, bufferIndex);

            HandleServerReconciliation();
        }

        private void HandleServerReconciliation()
        {
            if (!ShouldReconcile()) return;

            float positionError;
            StatePayload rewindState = default;
            int bufferIndex = _lastServerState.Tick % BUFFER_SIZE; //not enough data to reconcile
            
            if (bufferIndex - 1 < 0 ) return;

            rewindState = IsHost ? _serverStateBuffer.Get(bufferIndex - 1) : _lastServerState;
            positionError = Vector3.Distance(rewindState.Position, transform.position);
        }

        private bool ShouldReconcile()
        {
            throw new System.NotImplementedException();
        }

        private StatePayload ProcessMovement(InputPayload inputPayload)
        {
            Move(inputPayload.InputVector);
            
            return new StatePayload()
                   {
                       Position = transform.position,
                       Tick = inputPayload.Tick
                   };
        }
    #endregion

    #region Server
        private void HandleServerTick()
        {
            int bufferIndex = -1; //safeguard to throw exception

            while (_serverInputQueue.Count > 0)
            {
                InputPayload inputPayload = _serverInputQueue.Dequeue();
                bufferIndex = inputPayload.Tick % BUFFER_SIZE;
                
                StatePayload statePayload = SimulateMovement(inputPayload);
                _serverStateBuffer.Add(statePayload, bufferIndex);
            }
            
            if (bufferIndex == -1) return;
            SendToClientRpc(_serverStateBuffer.Get(bufferIndex));
        }

        private StatePayload SimulateMovement(InputPayload inputPayload)
        {
            Move(inputPayload.InputVector);

            return new StatePayload()
                   {
                       Tick = _networkTimer.CurrentTick,
                       Position = transform.position
                   };
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
#endregion

#region Movement
    private void Move(Vector3 movementInput)
    {
        _characterController.Move(movementInput * (_playerConfig.MovementSpeed * _tickTime));
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
        }
#endregion
    }
}