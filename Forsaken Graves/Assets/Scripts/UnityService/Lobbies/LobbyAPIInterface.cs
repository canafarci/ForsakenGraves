using System.Collections.Generic;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using Unity.Services.Lobbies;
using Unity.Services.Lobbies.Models;

namespace ForsakenGraves.UnityService.Lobbies
{
    public class LobbyAPIInterface
    {
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
    }
}