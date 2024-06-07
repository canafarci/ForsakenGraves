using System;
using ForsakenGraves.Connection.ConnectionStates;
using ForsakenGraves.Infrastructure.Dependencies;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.Serialization;
using VContainer;

namespace ForsakenGraves.Connection
{
    public class ConnectionStateManager : MonoBehaviour
    {
        [Inject] private NetworkManager _networkManager;
        //[Inject] private ConnectionStatesModel _connectionStatesModel;
        
        private ConnectionState _currentState;
        
        [SerializeField] private int _maxConnectedPlayers = 4;
        [SerializeField] private int _numberOfReconnectAttempts = 2;

        public int NumberOfReconnectAttempts => _numberOfReconnectAttempts;
        public int MaxConnectedPlayers => _maxConnectedPlayers;
        public NetworkManager NetworkManager => _networkManager;

        private void Awake()
        {
            DontDestroyOnLoad(gameObject);
        }
        
        private void Start()
        {
            _networkManager.OnClientConnectedCallback += OnClientConnectedCallback;
            _networkManager.OnClientDisconnectCallback += OnClientDisconnectCallback;
            _networkManager.OnServerStarted += OnServerStarted;
            _networkManager.ConnectionApprovalCallback += ApprovalCheck;
            _networkManager.OnTransportFailure += OnTransportFailure;
            _networkManager.OnServerStopped += OnServerStopped;
        }
        
        internal void ChangeState(ConnectionState nextState)
        {
            Debug.Log($"{name}: Changed connection state from {_currentState?.GetType().Name} to {nextState.GetType().Name}.");

            if (_currentState != null)
            {
                _currentState.Exit();
            }
            _currentState = nextState;
            _currentState.Enter();
        }
        
        //parameter not required
        private void OnServerStopped(bool _) => _currentState.OnServerStopped();

        private void OnTransportFailure() => _currentState.OnTransportFailure();

        private void ApprovalCheck(NetworkManager.ConnectionApprovalRequest request, NetworkManager.ConnectionApprovalResponse response) => _currentState.ApprovalCheck(request, response);

        private void OnServerStarted() => _currentState.OnServerStarted();

        private void OnClientDisconnectCallback(ulong clientId) => _currentState.OnClientDisconnectCallback(clientId);

        private void OnClientConnectedCallback(ulong clientId) => _currentState.OnClientConnectedCallback(clientId);
        
        public void StartHostLobby(string playerName)
        {
            _currentState.StartHostLobby(playerName);
        }

        public void StartClientLobby(string displayName)
        {
            _currentState.StartClientLobby(displayName);
        }
        
        public void RequestShutdown()
        {
            _currentState.OnUserRequestedShutdown();

        }

        void OnDestroy()
        {
            _networkManager.OnClientConnectedCallback -= OnClientConnectedCallback;
            _networkManager.OnClientDisconnectCallback -= OnClientDisconnectCallback;
            _networkManager.OnServerStarted -= OnServerStarted;
            _networkManager.ConnectionApprovalCallback -= ApprovalCheck;
            _networkManager.OnTransportFailure -= OnTransportFailure;
            _networkManager.OnServerStopped -= OnServerStopped;

        }
    }
}