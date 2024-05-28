using ForsakenGraves.GameState;
using Unity.Netcode;
using VContainer;

namespace ForsakenGraves.PreGame
{
    //holder for client side interactions inside pre game lobby
    public class ClientNetworkedPreGameLogic : NetworkBehaviour
    {
        [Inject] private ServerPreGameState _serverPreGameState;
        
        [ServerRpc(RequireOwnership = false)]
        public void OnReadyClickedServerRpc(ServerRpcParams serverRpcParams = default)
        {
            ulong clientId = serverRpcParams.Receive.SenderClientId;
            _serverPreGameState.OnPlayerReadyChanged(clientId);
        }
    }
}