using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using ForsakenGraves.Connection.Data;
using ForsakenGraves.Infrastructure;
using ForsakenGraves.UnityService.Lobbies;
using Unity.Netcode.Transports.UTP;
using Unity.Services.Authentication;
using Unity.Services.Core;
using Unity.Services.Relay;
using Unity.Services.Relay.Models;
using Unity.Networking.Transport.Relay;
using UnityEngine;

namespace ForsakenGraves.Connection
{
    public class RelayConnectionMethod
    {
        private readonly LobbyServiceFacade _lobbyServiceFacade;
        private readonly LocalLobby _localLobby;
        private readonly ConnectionStateManager _connectionManager;
        private readonly ProfileManager _profileManager;
        private readonly string _playerName;

        private const string DTLS_CONN_TYPE = "dtls";

        public RelayConnectionMethod(LobbyServiceFacade lobbyServiceFacade,
                                     LocalLobby localLobby,
                                     ConnectionStateManager connectionManager,
                                     ProfileManager profileManager,
                                     string playerName)
        {
            _lobbyServiceFacade = lobbyServiceFacade;
            _localLobby = localLobby;
            _connectionManager = connectionManager;
            _profileManager = profileManager;
            _playerName = playerName;
        }
        
        protected void SetConnectionPayload(string playerId, string playerName)
        {
            string payload = JsonUtility.ToJson(new ConnectionPayload()
                                                {
                                                    PlayerId = playerId,
                                                    PlayerName = playerName,
                                                    IsDebug = Debug.isDebugBuild
                                                });

            byte[] payloadBytes = System.Text.Encoding.UTF8.GetBytes(payload);

            _connectionManager.NetworkManager.NetworkConfig.ConnectionData = payloadBytes;
        }

        public async UniTask SetupHostConnectionAsync()
        {
            Debug.Log("Setting up Unity Relay host");

            SetConnectionPayload(GetPlayerId(), _playerName); // Need to set connection payload for host as well, as host is a client too

            // Create relay allocation
            Allocation hostAllocation = await RelayService.Instance.CreateAllocationAsync(_connectionManager.MaxConnectedPlayers, region: null);
            string joinCode = await RelayService.Instance.GetJoinCodeAsync(hostAllocation.AllocationId);

            Debug.Log($"server: connection data: {hostAllocation.ConnectionData[0]} {hostAllocation.ConnectionData[1]}, " +
                      $"allocation ID:{hostAllocation.AllocationId}, region:{hostAllocation.Region}");

            _localLobby.RelayJoinCode = joinCode;

            // next line enables lobby and relay services integration
            await _lobbyServiceFacade.UpdateLobbyDataAndUnlockAsync();
            await _lobbyServiceFacade.UpdatePlayerDataAsync(hostAllocation.AllocationIdBytes.ToString(), joinCode);

            // Setup UTP with relay connection info
            UnityTransport utp = (UnityTransport)_connectionManager.NetworkManager.NetworkConfig.NetworkTransport;
            utp.SetRelayServerData(new RelayServerData(hostAllocation, DTLS_CONN_TYPE)); // This is with DTLS enabled for a secure connection

            Debug.Log($"Created relay allocation with join code {_localLobby.RelayJoinCode}");
        }
        
        private string GetPlayerId()
        {
            if (UnityServices.State != ServicesInitializationState.Initialized)
            {
                return _profileManager.GetUniqueProfileID();
            }

            return AuthenticationService.Instance.IsSignedIn ? AuthenticationService.Instance.PlayerId : _profileManager.GetUniqueProfileID();
        }
    }
}