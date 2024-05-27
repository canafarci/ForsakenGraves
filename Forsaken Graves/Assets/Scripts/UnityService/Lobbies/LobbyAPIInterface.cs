using System.Collections.Generic;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using Unity.Services.Lobbies;
using Unity.Services.Lobbies.Models;

namespace ForsakenGraves.UnityService.Lobbies
{
    public class LobbyAPIInterface
    {
        private const int MAX_LOBBIES_TO_SHOW = 16;

        private readonly List<QueryFilter> _queryFilter = new() {
                                                            new QueryFilter(field: QueryFilter.FieldOptions.AvailableSlots,
                                                                            op: QueryFilter.OpOptions.GT,
                                                                            value: "0")
                                                                };
        
        private readonly List<QueryOrder> _queryOrder = new() {
                                                          new QueryOrder(asc: false,
                                                                         field: QueryOrder.FieldOptions.Created)
                                                              };

        // Filter for open lobbies only
        // Order by newest lobbies first

        public async UniTask<Lobby> CreateLobby(string instancePlayerId,
                                                string lobbyName,
                                                int maxConnectedPlayers,
                                                bool isPrivate,
                                                Dictionary<string, PlayerDataObject> hostUserData,
                                                Dictionary<string, DataObject> lobbyData)
        {
            CreateLobbyOptions createOptions = new CreateLobbyOptions
                                               {
                                                   IsPrivate = isPrivate,
                                                   IsLocked = true, // locking the lobby at creation to prevent other players from joining before it is ready
                                                   Player = new Player(id: instancePlayerId, data: hostUserData),
                                                   Data = lobbyData
                                               };

            return await LobbyService.Instance.CreateLobbyAsync(lobbyName, maxConnectedPlayers, createOptions);
        }

        public async void SendHeartbeatPing(string lobbyId) => await LobbyService.Instance.SendHeartbeatPingAsync(lobbyId);
        
        public async UniTask DeleteLobby(string lobbyId)
        {
            await LobbyService.Instance.DeleteLobbyAsync(lobbyId);
        }

        public async UniTask RemovePlayerFromLobby(string playerServiceID, string lobbyId)
        {
            try
            {
                await LobbyService.Instance.RemovePlayerAsync(lobbyId, playerServiceID);
            }
            catch (LobbyServiceException e) when (e is { Reason: LobbyExceptionReason.PlayerNotFound })
            {
                // If Player is not found, they have already left the lobby or have been kicked out. No need to throw here
            }
        }

        public async UniTask<Lobby> UpdateLobby(string lobbyId, Dictionary<string, DataObject> data, bool shouldLock)
        {
            UpdateLobbyOptions updateOptions = new UpdateLobbyOptions { Data = data, IsLocked = shouldLock };
            return await LobbyService.Instance.UpdateLobbyAsync(lobbyId, updateOptions);
        }

        public async UniTask<Lobby> UpdatePlayer(string lobbyId, string playerId, Dictionary<string, PlayerDataObject> data, string allocationId, string connectionInfo)
        {
            UpdatePlayerOptions updateOptions = new UpdatePlayerOptions
                                                {
                                                    Data = data,
                                                    AllocationId = allocationId,
                                                    ConnectionInfo = connectionInfo
                                                };
            return await LobbyService.Instance.UpdatePlayerAsync(lobbyId, playerId, updateOptions);
        }

        public async UniTask<ILobbyEvents> SubscribeToLobby(string lobbyId, LobbyEventCallbacks eventCallbacks)
        {
            return await LobbyService.Instance.SubscribeToLobbyEventsAsync(lobbyId, eventCallbacks);
        }

        public async UniTask<Lobby> QuickJoinLobby(string authId, Dictionary<string, PlayerDataObject> localUserData)
        {
            var joinRequest = new QuickJoinLobbyOptions
                              {
                                  Filter = _queryFilter,
                                  Player = new Player(id: authId, data: localUserData)
                              };

            return await LobbyService.Instance.QuickJoinLobbyAsync(joinRequest);        }
    }
}