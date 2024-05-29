using Cysharp.Threading.Tasks;
using ForsakenGraves.Connection;
using ForsakenGraves.Identifiers;
using ForsakenGraves.Infrastructure.SceneManagement.Messages;
using ForsakenGraves.PreGame;
using ForsakenGraves.PreGame.Data;
using MessagePipe;
using Unity.Netcode;
using VContainer;

namespace ForsakenGraves.GameState
{
    public class ServerPreGameState : NetworkBehaviour
    {
        [Inject] private ConnectionStateManager _connectionStateManager;
        [Inject] private PreGameNetwork _preGameNetwork;
        [Inject] private PlayerAvatarsSO _avatarsSO;
        [Inject] private IPublisher<LoadSceneMessage> _loadScenePublisher;
        
        public override void OnNetworkSpawn()
        {
            if (!NetworkManager.Singleton.IsServer)
            {
                enabled = false;
                return;
            }

            ulong clientID = _connectionStateManager.NetworkManager.LocalClient.ClientId;
            
            NetworkManager.Singleton.SceneManager.OnSceneEvent += OnClientLoadedScene;
            NetworkManager.Singleton.OnClientDisconnectCallback += OnClientDisconnectCallback;
            _preGameNetwork.PlayerLobbyDataNetworkList.OnListChanged += PlayerDataListChangedHandler;
        }

#region Check Player Ready
        private void PlayerDataListChangedHandler(NetworkListEvent<PlayerLobbyData> changeEvent)
        {
            if (CheckIfAllPlayersAreReady())
            {
                LoadGameplayScene();
            }
        }

        private async void LoadGameplayScene()
        {
            _preGameNetwork.IsLobbyLocked.Value = true;
            
            await UniTask.Delay(1000);
            _loadScenePublisher.Publish(new LoadSceneMessage(SceneIdentifier.PrototypeGameplayScene, true));
        }

        private bool CheckIfAllPlayersAreReady()
        {
            bool allPlayersAreReady = true;
            
            foreach (PlayerLobbyData pld in _preGameNetwork.PlayerLobbyDataNetworkList)
            {
                if (!pld.IsReady)
                {
                    allPlayersAreReady = false;
                    break;
                }
            }

            return allPlayersAreReady;
        }
#endregion

#region Add/Remove Player Lobby Data
        private void OnClientLoadedScene(SceneEvent sceneEvent)
        {
            if (sceneEvent.SceneEventType != SceneEventType.LoadComplete) return;
                    
            _preGameNetwork.AddNewPlayerData(sceneEvent.ClientId);
        }

        private void OnClientDisconnectCallback(ulong clientID)
        {
            _preGameNetwork.RemovePlayerData(clientID);
        }
#endregion

#region Server RPC follow-ups which change Network List
        //called from server rpc
        public void OnPlayerReadyChanged(ulong clientId)
        {
            (int playerIndex, PlayerLobbyData lobbyData) clientData = _preGameNetwork.GetPlayerLobbyData(clientId);
                    
            bool playerIsReady = clientData.lobbyData.IsReady;
            clientData.lobbyData.IsReady = !playerIsReady;

            _preGameNetwork.ChangeLobbyData(clientData.playerIndex, clientData.lobbyData);
        }
                
        //called from server rpc
        public void OnClientAvatarChanged(ulong clientId, int nextIndex)
        {
            (int playerIndex, PlayerLobbyData lobbyData) clientData = _preGameNetwork.GetPlayerLobbyData(clientId);
                    
            PlayerLobbyData changedLobbyData = clientData.lobbyData;
            changedLobbyData.AvatarIndex = nextIndex;
            _preGameNetwork.ChangeLobbyData(clientData.playerIndex, changedLobbyData);
        }
#endregion        
        

        public override void OnNetworkDespawn()
        {
            if (!NetworkManager.Singleton.IsServer)
            {
                enabled = false;
                return;
            }
            
            NetworkManager.Singleton.SceneManager.OnSceneEvent -= OnClientLoadedScene;
            NetworkManager.Singleton.OnClientDisconnectCallback -= OnClientDisconnectCallback;
            _preGameNetwork.PlayerLobbyDataNetworkList.OnListChanged -= PlayerDataListChangedHandler;

        }
    }
}