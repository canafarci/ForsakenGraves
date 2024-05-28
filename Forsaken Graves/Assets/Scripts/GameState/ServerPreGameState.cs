using System;
using System.Collections.Generic;
using ForsakenGraves.Connection;
using ForsakenGraves.PreGame.Signals;
using MessagePipe;
using Unity.Netcode;
using UnityEngine;
using VContainer;

namespace ForsakenGraves.GameState
{
    public class ServerPreGameState : NetworkBehaviour
    {
        //listened by ReadyPanelController
        [Inject] private IPublisher<PlayerReadyChangedMessage> _readyChangedPublisher;
        [Inject] private ConnectionStateManager _connectionStateManager;
        
        private Dictionary<ulong, bool> _clientReadyLookup = new();

        public override void OnNetworkSpawn()
        {
            base.OnNetworkSpawn();
            if (!IsServer) return;

            ulong clientID = _connectionStateManager.NetworkManager.LocalClient.ClientId;
            OnClientConnectedCallbackHandler(clientID);
            
            NetworkManager.Singleton.OnClientConnectedCallback += OnClientConnectedCallbackHandler;
            NetworkManager.Singleton.OnClientDisconnectCallback += OnClientDisconnectCallback;
        }

        private void OnClientDisconnectCallback(ulong clientID)
        {
            _clientReadyLookup.Remove(clientID);
        }

        private void OnClientConnectedCallbackHandler(ulong clientID)
        {
            _clientReadyLookup.Add(clientID, false);
            Debug.Log($"CLIENT with ID {clientID} connected!");
        }

        public void OnPlayerReadyChanged(ulong clientId)
        {
            if (!IsServer) return;
            if (!_clientReadyLookup.ContainsKey(clientId))
            {
                throw new Exception($"Player with clientID {clientId} is not inside the lookup!");
            }
            
            bool playerIsReady = _clientReadyLookup[clientId];
            _clientReadyLookup[clientId] = !playerIsReady;
            
            SendReadyButtonChangeRpc(clientId, _clientReadyLookup[clientId]);
        }
        
        [Rpc(SendTo.Everyone)]
        private void SendReadyButtonChangeRpc(ulong clientId, bool isReady)
        {
            _readyChangedPublisher.Publish(new PlayerReadyChangedMessage(isReady, clientId));
        }
    }
}