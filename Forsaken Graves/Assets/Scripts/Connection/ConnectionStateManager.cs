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
        [Inject] private RuntimeInjector _runtimeInjector;
        
        private ConnectionState _currentState;
        private OfflineState _offlineState;
        
        [SerializeField] private int _maxConnectedPlayers = 4;
        [SerializeField] private int _numberOfReconnectAttempts = 2;

        public int NumberOfReconnectAttempts => _numberOfReconnectAttempts;
        public int MaxConnectedPlayers => _maxConnectedPlayers;
        
        public void StartHostLobby(string displayName)
        {
            throw new System.NotImplementedException();
        }

        private void Awake()
        {
            DontDestroyOnLoad(gameObject);
            
            _offlineState = new OfflineState();
            _runtimeInjector.Inject(_offlineState);
            
            _currentState = _offlineState;
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
            Debug.Log($"{name}: Changed connection state from {_currentState.GetType().Name} to {nextState.GetType().Name}.");

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