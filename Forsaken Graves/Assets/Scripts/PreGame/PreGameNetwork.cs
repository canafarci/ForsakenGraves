using System;
using Cysharp.Threading.Tasks;
using ForsakenGraves.Gameplay.Data;
using ForsakenGraves.GameState;
using ForsakenGraves.PreGame.Data;
using Unity.Netcode;
using VContainer;

namespace ForsakenGraves.PreGame
{
    //holder for client side interactions inside pre game lobby
    public class PreGameNetwork : NetworkBehaviour
    {
        private NetworkList<PlayerLobbyData> _playerLobbyDataNetworkList;
        private NetworkVariable<bool> _isLobbyLocked = new NetworkVariable<bool>(false);

        public NetworkList<PlayerLobbyData> PlayerLobbyDataNetworkList => _playerLobbyDataNetworkList;
        public NetworkVariable<bool> IsLobbyLocked => _isLobbyLocked;

        public event Action<ulong> OnPlayerReadyChanged;
        public event Action<ulong, int> OnClientAvatarChanged;
        
        private void Awake()
        {
            _playerLobbyDataNetworkList = new NetworkList<PlayerLobbyData>();
        }

        public override void OnNetworkSpawn()
        {
            if (IsServer) return;
            _isLobbyLocked.OnValueChanged += OnLobbyLockedChanged;
        }

        private void OnLobbyLockedChanged(bool previousValue, bool newValue)
        {
            _isLobbyLocked.Value = newValue;
        }

        public void ChangeLobbyData(int networkListPlayerIndex, PlayerLobbyData clientLobbyData)
        {
            _playerLobbyDataNetworkList[networkListPlayerIndex] = clientLobbyData;
        }

        public (int playerIndex, PlayerLobbyData lobbyData) GetPlayerLobbyData(ulong clientID)
        {
            for (int i = 0; i < _playerLobbyDataNetworkList.Count; i++)
            {
                PlayerLobbyData playerLobbyData = _playerLobbyDataNetworkList[i];
                if (playerLobbyData.ClientID == clientID)
                    return (i, playerLobbyData);
            }

            throw new Exception($"No data associated with {clientID} is inside the network list!");
        }
        
        [Rpc(SendTo.Server)]
        public void OnReadyClickedServerRpc(RpcParams rpcParams = default)
        {
            ulong clientId = rpcParams.Receive.SenderClientId;
            OnPlayerReadyChanged?.Invoke(clientId);
        }
        
        [Rpc(SendTo.Server)]
        public void ChangeAvatarServerRpc(int avatarIndex, RpcParams rpcParams = default)
        {
            ulong clientId = rpcParams.Receive.SenderClientId;
            OnClientAvatarChanged?.Invoke(clientId, avatarIndex);
        }

        public void AddNewPlayerData(ulong clientID)
        {
            _playerLobbyDataNetworkList.Add(new PlayerLobbyData(clientID));
        }

        public void RemovePlayerData(ulong clientId)
        {
            (int playerIndex, PlayerLobbyData lobbyData) clientData = GetPlayerLobbyData(clientId);
            _playerLobbyDataNetworkList.RemoveAt(clientData.playerIndex);
        }
        
        public override void OnNetworkDespawn()
        {
            if (IsServer) return;
            _isLobbyLocked.OnValueChanged -= OnLobbyLockedChanged;
        }
    }
}