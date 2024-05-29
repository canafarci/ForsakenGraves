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
        [Inject] private ServerPreGameState _serverPreGameState;
        private NetworkList<PlayerLobbyData> _playerLobbyDataNetworkList;
        public NetworkList<PlayerLobbyData> PlayerLobbyDataNetworkList => _playerLobbyDataNetworkList;

        private void Awake()
        {
            _playerLobbyDataNetworkList = new NetworkList<PlayerLobbyData>();
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
        
        [Rpc(SendTo.Server, RequireOwnership = false)]
        public void OnReadyClickedServerRpc(RpcParams rpcParams = default)
        {
            ulong clientId = rpcParams.Receive.SenderClientId;
            _serverPreGameState.OnPlayerReadyChanged(clientId);
        }
        
        [Rpc(SendTo.Server, RequireOwnership = false)]
        public void ChangeAvatarServerRpc(int avatarIndex, RpcParams rpcParams = default)
        {
            ulong clientId = rpcParams.Receive.SenderClientId;
            _serverPreGameState.OnClientAvatarChanged(clientId, avatarIndex);
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


    }
}