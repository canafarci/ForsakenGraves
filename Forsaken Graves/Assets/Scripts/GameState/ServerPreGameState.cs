using System;
using System.Collections.Generic;
using ForsakenGraves.Connection;
using ForsakenGraves.PreGame;
using ForsakenGraves.PreGame.Data;
using MessagePipe;
using Unity.Netcode;
using UnityEngine;
using VContainer;

namespace ForsakenGraves.GameState
{
    public class ServerPreGameState : NetworkBehaviour
    {
        [Inject] private ConnectionStateManager _connectionStateManager;
        [Inject] private PreGameNetwork _preGameNetwork;
        
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
        }

        private void OnClientLoadedScene(SceneEvent sceneEvent)
        {
            if (sceneEvent.SceneEventType != SceneEventType.LoadComplete) return;
            
            _preGameNetwork.AddNewPlayerData(sceneEvent.ClientId);
        }

        private void OnClientDisconnectCallback(ulong clientID)
        {
            _preGameNetwork.RemovePlayerData(clientID);
        }
        
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
            throw new NotImplementedException();
        }

        public override void OnNetworkDespawn()
        {
            if (!NetworkManager.Singleton.IsServer)
            {
                enabled = false;
                return;
            }
            
            NetworkManager.Singleton.SceneManager.OnSceneEvent -= OnClientLoadedScene;
            NetworkManager.Singleton.OnClientDisconnectCallback -= OnClientDisconnectCallback;
        }
    }
}